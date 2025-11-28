using IncidentsTI.Application.DTOs.Statistics;
using IncidentsTI.Application.Queries;
using IncidentsTI.Domain.Enums;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

public class GetServiceStatisticsQueryHandler : IRequestHandler<GetServiceStatisticsQuery, List<ServiceStatDto>>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IServiceRepository _serviceRepository;

    public GetServiceStatisticsQueryHandler(
        IIncidentRepository incidentRepository,
        IServiceRepository serviceRepository)
    {
        _incidentRepository = incidentRepository;
        _serviceRepository = serviceRepository;
    }

    public async Task<List<ServiceStatDto>> Handle(GetServiceStatisticsQuery request, CancellationToken cancellationToken)
    {
        var endDate = request.EndDate ?? DateTime.UtcNow;
        var startDate = request.StartDate ?? DateTime.MinValue;

        var allIncidents = (await _incidentRepository.GetAllAsync())
            .Where(i => i.CreatedAt >= startDate && i.CreatedAt <= endDate)
            .ToList();

        var services = await _serviceRepository.GetAllAsync();
        
        if (!request.IncludeInactive)
        {
            services = services.Where(s => s.IsActive);
        }

        var serviceDict = services.ToDictionary(s => s.Id, s => s);
        var total = allIncidents.Count;

        var result = services.Select(service => {
            var serviceIncidents = allIncidents.Where(i => i.ServiceId == service.Id).ToList();
            var resolved = serviceIncidents.Where(i => i.ResolvedAt.HasValue).ToList();
            var avgTime = resolved.Any() 
                ? resolved.Average(i => (i.ResolvedAt!.Value - i.CreatedAt).TotalHours) 
                : 0;

            return new ServiceStatDto
            {
                ServiceId = service.Id,
                ServiceName = service.Name,
                Category = service.Category.ToString(),
                TotalIncidents = serviceIncidents.Count,
                OpenIncidents = serviceIncidents.Count(i => 
                    i.Status == IncidentStatus.Open || 
                    i.Status == IncidentStatus.InProgress || 
                    i.Status == IncidentStatus.Escalated),
                ResolvedIncidents = serviceIncidents.Count(i => 
                    i.Status == IncidentStatus.Resolved || 
                    i.Status == IncidentStatus.Closed),
                AverageResolutionTimeHours = Math.Round(avgTime, 2),
                Percentage = total > 0 ? Math.Round((decimal)serviceIncidents.Count / total * 100, 1) : 0
            };
        })
        .OrderByDescending(s => s.TotalIncidents)
        .ToList();

        return result;
    }
}
