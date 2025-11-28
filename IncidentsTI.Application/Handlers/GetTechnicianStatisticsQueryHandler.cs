using IncidentsTI.Application.DTOs.Statistics;
using IncidentsTI.Application.Queries;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Enums;
using IncidentsTI.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IncidentsTI.Application.Handlers;

public class GetTechnicianStatisticsQueryHandler : IRequestHandler<GetTechnicianStatisticsQuery, List<TechnicianStatDto>>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetTechnicianStatisticsQueryHandler(
        IIncidentRepository incidentRepository,
        UserManager<ApplicationUser> userManager)
    {
        _incidentRepository = incidentRepository;
        _userManager = userManager;
    }

    public async Task<List<TechnicianStatDto>> Handle(GetTechnicianStatisticsQuery request, CancellationToken cancellationToken)
    {
        var endDate = request.EndDate ?? DateTime.UtcNow;
        var startDate = request.StartDate ?? DateTime.MinValue;

        var allIncidents = (await _incidentRepository.GetAllAsync())
            .Where(i => i.CreatedAt >= startDate && i.CreatedAt <= endDate)
            .Where(i => i.AssignedToId != null)
            .ToList();

        if (!allIncidents.Any()) return new List<TechnicianStatDto>();

        // Obtener todos los técnicos y administradores
        var technicians = await _userManager.GetUsersInRoleAsync("Technician");
        var admins = await _userManager.GetUsersInRoleAsync("Administrator");
        
        var allTechUsers = technicians.Union(admins).DistinctBy(u => u.Id).ToList();
        var technicianDict = allTechUsers.ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}");

        var result = allIncidents
            .GroupBy(i => i.AssignedToId!)
            .Select(g => {
                var resolved = g.Where(i => 
                    i.Status == IncidentStatus.Resolved || 
                    i.Status == IncidentStatus.Closed).ToList();
                    
                var resolutionRate = g.Any() ? (decimal)resolved.Count / g.Count() * 100 : 0;
                
                var avgTime = resolved.Any() && resolved.All(i => i.ResolvedAt.HasValue)
                    ? resolved.Average(i => (i.ResolvedAt!.Value - i.CreatedAt).TotalHours)
                    : 0;

                return new TechnicianStatDto
                {
                    TechnicianId = g.Key,
                    TechnicianName = technicianDict.GetValueOrDefault(g.Key, "Técnico desconocido"),
                    TotalAssigned = g.Count(),
                    OpenIncidents = g.Count(i => i.Status == IncidentStatus.Open),
                    InProgressIncidents = g.Count(i => i.Status == IncidentStatus.InProgress),
                    ResolvedIncidents = g.Count(i => i.Status == IncidentStatus.Resolved),
                    ClosedIncidents = g.Count(i => i.Status == IncidentStatus.Closed),
                    AverageResolutionTimeHours = Math.Round(avgTime, 2),
                    ResolutionRate = Math.Round(resolutionRate, 1)
                };
            })
            .OrderByDescending(t => t.TotalAssigned)
            .ToList();

        return result;
    }
}
