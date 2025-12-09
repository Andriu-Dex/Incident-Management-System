using IncidentsTI.Application.DTOs;
using IncidentsTI.Application.Queries;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Enums;
using IncidentsTI.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IncidentsTI.Application.Handlers;

/// <summary>
/// Handler para obtener los incidentes que el usuario escaló.
/// Estos incidentes se muestran para seguimiento en modo solo lectura.
/// </summary>
public class GetEscalatedByMeIncidentsQueryHandler 
    : IRequestHandler<GetEscalatedByMeIncidentsQuery, IEnumerable<IncidentDto>>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IEscalationLevelRepository _escalationLevelRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetEscalatedByMeIncidentsQueryHandler(
        IIncidentRepository incidentRepository,
        IServiceRepository serviceRepository,
        IEscalationLevelRepository escalationLevelRepository,
        UserManager<ApplicationUser> userManager)
    {
        _incidentRepository = incidentRepository;
        _serviceRepository = serviceRepository;
        _escalationLevelRepository = escalationLevelRepository;
        _userManager = userManager;
    }

    public async Task<IEnumerable<IncidentDto>> Handle(
        GetEscalatedByMeIncidentsQuery request, 
        CancellationToken cancellationToken)
    {
        // Obtener todos los incidentes
        var allIncidents = await _incidentRepository.GetAllAsync();
        
        // Filtrar incidentes que el usuario escaló y que aún no están cerrados/resueltos
        // (solo mostramos los que aún están activos para seguimiento)
        var escalatedByMe = allIncidents
            .Where(i => 
                i.EscalatedByUserId == request.UserId &&
                i.Status != IncidentStatus.Resolved &&
                i.Status != IncidentStatus.Closed)
            .OrderByDescending(i => i.UpdatedAt ?? i.CreatedAt);

        var result = new List<IncidentDto>();

        foreach (var incident in escalatedByMe)
        {
            var creator = await _userManager.FindByIdAsync(incident.UserId);
            var assignedTo = !string.IsNullOrEmpty(incident.AssignedToId) 
                ? await _userManager.FindByIdAsync(incident.AssignedToId) 
                : null;
            var service = await _serviceRepository.GetByIdAsync(incident.ServiceId);
            
            // Obtener el nombre del nivel de escalamiento
            string? escalationLevelName = null;
            if (incident.CurrentEscalationLevelId.HasValue)
            {
                var level = await _escalationLevelRepository.GetByIdAsync(incident.CurrentEscalationLevelId.Value);
                escalationLevelName = level?.Name;
            }
            
            // Obtener el nombre del usuario que escaló (en este caso es el mismo usuario actual)
            var escalatedBy = !string.IsNullOrEmpty(incident.EscalatedByUserId) 
                ? await _userManager.FindByIdAsync(incident.EscalatedByUserId) 
                : null;

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
                UserName = creator != null ? $"{creator.FirstName} {creator.LastName}" : "",
                UserEmail = creator?.Email ?? "",
                AssignedToId = incident.AssignedToId,
                AssignedToName = assignedTo != null ? $"{assignedTo.FirstName} {assignedTo.LastName}" : null,
                CreatedAt = incident.CreatedAt,
                UpdatedAt = incident.UpdatedAt,
                CurrentEscalationLevelId = incident.CurrentEscalationLevelId,
                CurrentEscalationLevelName = escalationLevelName,
                EscalatedByUserId = incident.EscalatedByUserId,
                EscalatedByUserName = escalatedBy != null ? $"{escalatedBy.FirstName} {escalatedBy.LastName}" : null
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
