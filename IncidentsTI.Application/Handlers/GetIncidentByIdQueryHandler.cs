using IncidentsTI.Application.DTOs;
using IncidentsTI.Application.Queries;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IncidentsTI.Application.Handlers;

public class GetIncidentByIdQueryHandler : IRequestHandler<GetIncidentByIdQuery, IncidentDto?>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetIncidentByIdQueryHandler(
        IIncidentRepository incidentRepository,
        IServiceRepository serviceRepository,
        UserManager<ApplicationUser> userManager)
    {
        _incidentRepository = incidentRepository;
        _serviceRepository = serviceRepository;
        _userManager = userManager;
    }

    public async Task<IncidentDto?> Handle(GetIncidentByIdQuery request, CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetByIdAsync(request.Id);
        
        if (incident == null)
            return null;

        var user = await _userManager.FindByIdAsync(incident.UserId);
        var service = await _serviceRepository.GetByIdAsync(incident.ServiceId);
        ApplicationUser? assignedTo = null;
        
        if (!string.IsNullOrEmpty(incident.AssignedToId))
        {
            assignedTo = await _userManager.FindByIdAsync(incident.AssignedToId);
        }

        return new IncidentDto
        {
            Id = incident.Id,
            TicketNumber = incident.TicketNumber,
            UserId = incident.UserId,
            UserName = user != null ? $"{user.FirstName} {user.LastName}" : "",
            UserEmail = user?.Email ?? "",
            ServiceId = incident.ServiceId,
            ServiceName = service?.Name ?? "",
            ServiceCategory = service?.Category.ToString() ?? "",
            Type = incident.Type,
            TypeName = GetTypeName(incident.Type),
            Priority = incident.Priority,
            PriorityName = GetPriorityName(incident.Priority),
            Status = incident.Status,
            StatusName = GetStatusName(incident.Status),
            Title = incident.Title,
            Description = incident.Description,
            AssignedToId = incident.AssignedToId,
            AssignedToName = assignedTo != null ? $"{assignedTo.FirstName} {assignedTo.LastName}" : null,
            AssignedToEmail = assignedTo?.Email,
            CreatedAt = incident.CreatedAt,
            UpdatedAt = incident.UpdatedAt
        };
    }

    private string GetTypeName(Domain.Enums.IncidentType type)
    {
        return type switch
        {
            Domain.Enums.IncidentType.Failure => "Falla",
            Domain.Enums.IncidentType.Query => "Consulta",
            Domain.Enums.IncidentType.Request => "Requerimiento",
            _ => type.ToString()
        };
    }

    private string GetPriorityName(Domain.Enums.IncidentPriority priority)
    {
        return priority switch
        {
            Domain.Enums.IncidentPriority.Low => "Baja",
            Domain.Enums.IncidentPriority.Medium => "Media",
            Domain.Enums.IncidentPriority.High => "Alta",
            Domain.Enums.IncidentPriority.Critical => "Crítica",
            _ => priority.ToString()
        };
    }

    private string GetStatusName(Domain.Enums.IncidentStatus status)
    {
        return status switch
        {
            Domain.Enums.IncidentStatus.Open => "Abierto",
            Domain.Enums.IncidentStatus.InProgress => "En Progreso",
            Domain.Enums.IncidentStatus.Escalated => "Escalado",
            Domain.Enums.IncidentStatus.Resolved => "Resuelto",
            Domain.Enums.IncidentStatus.Closed => "Cerrado",
            _ => status.ToString()
        };
    }
}
