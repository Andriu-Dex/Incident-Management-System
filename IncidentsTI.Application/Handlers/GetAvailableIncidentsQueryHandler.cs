using IncidentsTI.Application.DTOs;
using IncidentsTI.Application.Queries;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Enums;
using IncidentsTI.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IncidentsTI.Application.Handlers;

/// <summary>
/// Handler para obtener los incidentes disponibles para reclamar
/// </summary>
public class GetAvailableIncidentsQueryHandler : IRequestHandler<GetAvailableIncidentsQuery, IEnumerable<IncidentDto>>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAvailableIncidentsQueryHandler(
        IIncidentRepository incidentRepository,
        IServiceRepository serviceRepository,
        UserManager<ApplicationUser> userManager)
    {
        _incidentRepository = incidentRepository;
        _serviceRepository = serviceRepository;
        _userManager = userManager;
    }

    public async Task<IEnumerable<IncidentDto>> Handle(GetAvailableIncidentsQuery request, CancellationToken cancellationToken)
    {
        // Obtener todos los incidentes en estado Open y sin asignar
        var allIncidents = await _incidentRepository.GetAllAsync();
        var availableIncidents = allIncidents
            .Where(i => i.Status == IncidentStatus.Open && string.IsNullOrEmpty(i.AssignedToId))
            .OrderByDescending(i => i.Priority) // Prioridad más alta primero
            .ThenBy(i => i.CreatedAt); // Más antiguos primero

        var result = new List<IncidentDto>();

        foreach (var incident in availableIncidents)
        {
            var user = await _userManager.FindByIdAsync(incident.UserId);
            var service = await _serviceRepository.GetByIdAsync(incident.ServiceId);

            result.Add(new IncidentDto
            {
                Id = incident.Id,
                TicketNumber = incident.TicketNumber,
                Title = incident.Title,
                Description = incident.Description,
                ServiceId = incident.ServiceId,
                ServiceName = service?.Name ?? "",
                Type = incident.Type,
                TypeName = GetTypeName(incident.Type),
                Status = incident.Status,
                StatusName = GetStatusName(incident.Status),
                Priority = incident.Priority,
                PriorityName = GetPriorityName(incident.Priority),
                UserId = incident.UserId,
                UserName = user != null ? $"{user.FirstName} {user.LastName}" : "",
                UserEmail = user?.Email ?? "",
                CreatedAt = incident.CreatedAt
            });
        }

        return result;
    }

    private static string GetTypeName(IncidentType type) => type switch
    {
        IncidentType.Failure => "Falla",
        IncidentType.Query => "Consulta",
        IncidentType.Request => "Requerimiento",
        _ => type.ToString()
    };

    private static string GetStatusName(IncidentStatus status) => status switch
    {
        IncidentStatus.Open => "Abierto",
        IncidentStatus.InProgress => "En Progreso",
        IncidentStatus.Escalated => "Escalado",
        IncidentStatus.Resolved => "Resuelto",
        IncidentStatus.Closed => "Cerrado",
        _ => status.ToString()
    };

    private static string GetPriorityName(IncidentPriority priority) => priority switch
    {
        IncidentPriority.Low => "Baja",
        IncidentPriority.Medium => "Media",
        IncidentPriority.High => "Alta",
        IncidentPriority.Critical => "Crítica",
        _ => priority.ToString()
    };
}
