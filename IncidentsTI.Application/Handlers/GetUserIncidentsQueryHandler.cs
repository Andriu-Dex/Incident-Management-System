using IncidentsTI.Application.DTOs;
using IncidentsTI.Application.Queries;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IncidentsTI.Application.Handlers;

public class GetUserIncidentsQueryHandler : IRequestHandler<GetUserIncidentsQuery, IEnumerable<IncidentListDto>>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IKnowledgeArticleRepository _articleRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUserIncidentsQueryHandler(
        IIncidentRepository incidentRepository,
        IServiceRepository serviceRepository,
        IKnowledgeArticleRepository articleRepository,
        UserManager<ApplicationUser> userManager)
    {
        _incidentRepository = incidentRepository;
        _serviceRepository = serviceRepository;
        _articleRepository = articleRepository;
        _userManager = userManager;
    }

    public async Task<IEnumerable<IncidentListDto>> Handle(GetUserIncidentsQuery request, CancellationToken cancellationToken)
    {
        var incidents = await _incidentRepository.GetByUserIdAsync(request.UserId);
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

            // Verificar si tiene artículos vinculados
            var linkedArticles = await _articleRepository.GetIncidentLinksAsync(incident.Id);

            result.Add(new IncidentListDto
            {
                Id = incident.Id,
                TicketNumber = incident.TicketNumber,
                Title = incident.Title,
                ServiceId = incident.ServiceId,
                ServiceName = service?.Name ?? "",
                Type = incident.Type,
                TypeName = GetTypeName(incident.Type),
                Status = incident.Status,
                StatusName = GetStatusName(incident.Status),
                Priority = incident.Priority,
                PriorityName = GetPriorityName(incident.Priority),
                UserName = user != null ? $"{user.FirstName} {user.LastName}" : "",
                AssignedToId = incident.AssignedToId,
                AssignedToName = assignedTo != null ? $"{assignedTo.FirstName} {assignedTo.LastName}" : null,
                CreatedAt = incident.CreatedAt,
                HasLinkedArticles = linkedArticles.Any()
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
