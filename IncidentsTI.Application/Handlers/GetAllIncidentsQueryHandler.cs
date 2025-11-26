using IncidentsTI.Application.DTOs;
using IncidentsTI.Application.Queries;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IncidentsTI.Application.Handlers;

public class GetAllIncidentsQueryHandler : IRequestHandler<GetAllIncidentsQuery, IEnumerable<IncidentListDto>>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAllIncidentsQueryHandler(
        IIncidentRepository incidentRepository,
        IServiceRepository serviceRepository,
        UserManager<ApplicationUser> userManager)
    {
        _incidentRepository = incidentRepository;
        _serviceRepository = serviceRepository;
        _userManager = userManager;
    }

    public async Task<IEnumerable<IncidentListDto>> Handle(GetAllIncidentsQuery request, CancellationToken cancellationToken)
    {
        var incidents = await _incidentRepository.GetAllAsync();
        
        // Aplicar filtros
        if (request.StatusFilter.HasValue)
        {
            incidents = incidents.Where(i => i.Status == request.StatusFilter.Value);
        }
        
        if (request.ServiceIdFilter.HasValue)
        {
            incidents = incidents.Where(i => i.ServiceId == request.ServiceIdFilter.Value);
        }
        
        if (request.PriorityFilter.HasValue)
        {
            incidents = incidents.Where(i => i.Priority == request.PriorityFilter.Value);
        }

        var result = new List<IncidentListDto>();

        foreach (var incident in incidents)
        {
            var user = await _userManager.FindByIdAsync(incident.UserId);
            var service = await _serviceRepository.GetByIdAsync(incident.ServiceId);
            ApplicationUser? assignedTo = null;
            
            if (!string.IsNullOrEmpty(incident.AssignedToId))
            {
                assignedTo = await _userManager.FindByIdAsync(incident.AssignedToId);
            }

            result.Add(new IncidentListDto
            {
                Id = incident.Id,
                TicketNumber = incident.TicketNumber,
                Title = incident.Title,
                ServiceName = service?.Name ?? "",
                Status = incident.Status,
                StatusName = GetStatusName(incident.Status),
                Priority = incident.Priority,
                PriorityName = GetPriorityName(incident.Priority),
                UserName = user != null ? $"{user.FirstName} {user.LastName}" : "",
                AssignedToId = incident.AssignedToId,
                AssignedToName = assignedTo != null ? $"{assignedTo.FirstName} {assignedTo.LastName}" : null,
                CreatedAt = incident.CreatedAt
            });
        }

        return result.OrderByDescending(i => i.CreatedAt);
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
