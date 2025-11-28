using IncidentsTI.Application.DTOs.Statistics;
using IncidentsTI.Application.Queries;
using IncidentsTI.Domain.Enums;
using IncidentsTI.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using IncidentsTI.Domain.Entities;

namespace IncidentsTI.Application.Handlers;

public class GetDashboardStatisticsQueryHandler : IRequestHandler<GetDashboardStatisticsQuery, DashboardStatisticsDto>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetDashboardStatisticsQueryHandler(
        IIncidentRepository incidentRepository,
        IServiceRepository serviceRepository,
        UserManager<ApplicationUser> userManager)
    {
        _incidentRepository = incidentRepository;
        _serviceRepository = serviceRepository;
        _userManager = userManager;
    }

    public async Task<DashboardStatisticsDto> Handle(GetDashboardStatisticsQuery request, CancellationToken cancellationToken)
    {
        // Definir período de análisis
        var endDate = request.EndDate ?? DateTime.UtcNow;
        var startDate = request.StartDate ?? endDate.AddDays(-30);

        // Obtener todos los incidentes
        var allIncidents = (await _incidentRepository.GetAllAsync()).ToList();
        
        // Filtrar por período
        var incidentsInPeriod = allIncidents
            .Where(i => i.CreatedAt >= startDate && i.CreatedAt <= endDate)
            .ToList();

        var result = new DashboardStatisticsDto
        {
            StartDate = startDate,
            EndDate = endDate,
            
            // KPIs principales (todos los incidentes, no solo del período)
            TotalIncidents = allIncidents.Count,
            OpenIncidents = allIncidents.Count(i => i.Status == IncidentStatus.Open),
            InProgressIncidents = allIncidents.Count(i => i.Status == IncidentStatus.InProgress),
            ResolvedIncidents = allIncidents.Count(i => i.Status == IncidentStatus.Resolved),
            ClosedIncidents = allIncidents.Count(i => i.Status == IncidentStatus.Closed),
            EscalatedIncidents = allIncidents.Count(i => i.Status == IncidentStatus.Escalated),
            UnassignedIncidents = allIncidents.Count(i => i.AssignedToId == null && 
                i.Status != IncidentStatus.Closed && i.Status != IncidentStatus.Resolved),
            
            // Métricas de tiempo
            AverageResolutionTimeHours = CalculateAverageResolutionTime(allIncidents),
            AverageFirstResponseTimeHours = CalculateAverageFirstResponseTime(allIncidents),
            
            // Estadísticas por categorías
            IncidentsByStatus = GetStatusStatistics(allIncidents),
            IncidentsByPriority = GetPriorityStatistics(allIncidents),
            IncidentsByType = GetTypeStatistics(allIncidents),
            IncidentsByService = await GetServiceStatisticsAsync(allIncidents),
            IncidentsByTechnician = await GetTechnicianStatisticsAsync(allIncidents),
            
            // Tendencias
            DailyTrend = GetDailyTrend(incidentsInPeriod, startDate, endDate),
            MonthlyTrend = GetMonthlyTrend(allIncidents)
        };

        return result;
    }

    private double CalculateAverageResolutionTime(List<Incident> incidents)
    {
        var resolvedIncidents = incidents
            .Where(i => i.ResolvedAt.HasValue && i.CreatedAt != default)
            .ToList();

        if (!resolvedIncidents.Any()) return 0;

        var totalHours = resolvedIncidents
            .Sum(i => (i.ResolvedAt!.Value - i.CreatedAt).TotalHours);

        return Math.Round(totalHours / resolvedIncidents.Count, 2);
    }

    private double CalculateAverageFirstResponseTime(List<Incident> incidents)
    {
        // Tiempo promedio hasta que se asigna un técnico
        var assignedIncidents = incidents
            .Where(i => i.AssignedToId != null)
            .ToList();

        if (!assignedIncidents.Any()) return 0;

        // Aproximamos usando el tiempo entre creación y primera actualización
        var totalHours = assignedIncidents
            .Where(i => i.UpdatedAt.HasValue)
            .Sum(i => (i.UpdatedAt!.Value - i.CreatedAt).TotalHours);

        var count = assignedIncidents.Count(i => i.UpdatedAt.HasValue);
        
        return count > 0 ? Math.Round(totalHours / count, 2) : 0;
    }

    private List<StatusStatDto> GetStatusStatistics(List<Incident> incidents)
    {
        var total = incidents.Count;
        if (total == 0) return new List<StatusStatDto>();

        var statusGroups = incidents
            .GroupBy(i => i.Status)
            .Select(g => new StatusStatDto
            {
                Status = g.Key.ToString(),
                StatusDisplay = GetStatusDisplayName(g.Key),
                Count = g.Count(),
                Percentage = Math.Round((decimal)g.Count() / total * 100, 1),
                Color = GetStatusColor(g.Key)
            })
            .OrderBy(s => (int)Enum.Parse<IncidentStatus>(s.Status))
            .ToList();

        return statusGroups;
    }

    private List<PriorityStatDto> GetPriorityStatistics(List<Incident> incidents)
    {
        var total = incidents.Count;
        if (total == 0) return new List<PriorityStatDto>();

        return incidents
            .GroupBy(i => i.Priority)
            .Select(g => new PriorityStatDto
            {
                Priority = g.Key.ToString(),
                PriorityDisplay = GetPriorityDisplayName(g.Key),
                Count = g.Count(),
                Percentage = Math.Round((decimal)g.Count() / total * 100, 1),
                Color = GetPriorityColor(g.Key)
            })
            .OrderByDescending(p => (int)Enum.Parse<IncidentPriority>(p.Priority))
            .ToList();
    }

    private List<TypeStatDto> GetTypeStatistics(List<Incident> incidents)
    {
        var total = incidents.Count;
        if (total == 0) return new List<TypeStatDto>();

        return incidents
            .GroupBy(i => i.Type)
            .Select(g => new TypeStatDto
            {
                Type = g.Key.ToString(),
                TypeDisplay = GetTypeDisplayName(g.Key),
                Count = g.Count(),
                Percentage = Math.Round((decimal)g.Count() / total * 100, 1),
                Color = GetTypeColor(g.Key)
            })
            .ToList();
    }

    private async Task<List<ServiceStatDto>> GetServiceStatisticsAsync(List<Incident> incidents)
    {
        var total = incidents.Count;
        if (total == 0) return new List<ServiceStatDto>();

        var services = await _serviceRepository.GetAllAsync();
        var serviceDict = services.ToDictionary(s => s.Id, s => s);

        return incidents
            .GroupBy(i => i.ServiceId)
            .Select(g => {
                var service = serviceDict.GetValueOrDefault(g.Key);
                var resolved = g.Where(i => i.ResolvedAt.HasValue).ToList();
                var avgTime = resolved.Any() 
                    ? resolved.Average(i => (i.ResolvedAt!.Value - i.CreatedAt).TotalHours) 
                    : 0;

                return new ServiceStatDto
                {
                    ServiceId = g.Key,
                    ServiceName = service?.Name ?? "Servicio desconocido",
                    Category = service?.Category.ToString() ?? "Other",
                    TotalIncidents = g.Count(),
                    OpenIncidents = g.Count(i => i.Status == IncidentStatus.Open || i.Status == IncidentStatus.InProgress),
                    ResolvedIncidents = g.Count(i => i.Status == IncidentStatus.Resolved || i.Status == IncidentStatus.Closed),
                    AverageResolutionTimeHours = Math.Round(avgTime, 2),
                    Percentage = Math.Round((decimal)g.Count() / total * 100, 1)
                };
            })
            .OrderByDescending(s => s.TotalIncidents)
            .ToList();
    }

    private async Task<List<TechnicianStatDto>> GetTechnicianStatisticsAsync(List<Incident> incidents)
    {
        var assignedIncidents = incidents.Where(i => i.AssignedToId != null).ToList();
        
        if (!assignedIncidents.Any()) return new List<TechnicianStatDto>();

        var technicianIds = assignedIncidents.Select(i => i.AssignedToId!).Distinct().ToList();
        var technicians = new Dictionary<string, string>();
        
        foreach (var id in technicianIds)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                technicians[id] = $"{user.FirstName} {user.LastName}";
            }
        }

        return assignedIncidents
            .GroupBy(i => i.AssignedToId!)
            .Select(g => {
                var resolved = g.Where(i => i.Status == IncidentStatus.Resolved || i.Status == IncidentStatus.Closed).ToList();
                var resolutionRate = g.Any() ? (decimal)resolved.Count / g.Count() * 100 : 0;
                var avgTime = resolved.Any() && resolved.All(i => i.ResolvedAt.HasValue)
                    ? resolved.Average(i => (i.ResolvedAt!.Value - i.CreatedAt).TotalHours)
                    : 0;

                return new TechnicianStatDto
                {
                    TechnicianId = g.Key,
                    TechnicianName = technicians.GetValueOrDefault(g.Key, "Técnico desconocido"),
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
    }

    private List<TrendDataDto> GetDailyTrend(List<Incident> incidents, DateTime startDate, DateTime endDate)
    {
        var trend = new List<TrendDataDto>();
        var currentDate = startDate.Date;

        while (currentDate <= endDate.Date)
        {
            var dayIncidents = incidents.Where(i => i.CreatedAt.Date == currentDate).ToList();
            var allIncidentsUpToDate = incidents.Where(i => i.CreatedAt.Date <= currentDate).ToList();

            trend.Add(new TrendDataDto
            {
                Date = currentDate,
                Label = currentDate.ToString("dd/MM"),
                Created = dayIncidents.Count,
                Resolved = incidents.Count(i => i.ResolvedAt?.Date == currentDate),
                Closed = incidents.Count(i => i.Status == IncidentStatus.Closed && i.UpdatedAt?.Date == currentDate),
                Open = allIncidentsUpToDate.Count(i => i.Status == IncidentStatus.Open || i.Status == IncidentStatus.InProgress)
            });

            currentDate = currentDate.AddDays(1);
        }

        return trend;
    }

    private List<TrendDataDto> GetMonthlyTrend(List<Incident> incidents)
    {
        var today = DateTime.UtcNow;
        var trend = new List<TrendDataDto>();

        for (int i = 11; i >= 0; i--)
        {
            var monthStart = new DateTime(today.Year, today.Month, 1).AddMonths(-i);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);
            
            var monthIncidents = incidents.Where(i => 
                i.CreatedAt >= monthStart && i.CreatedAt <= monthEnd).ToList();

            trend.Add(new TrendDataDto
            {
                Date = monthStart,
                Label = monthStart.ToString("MMM yyyy"),
                Created = monthIncidents.Count,
                Resolved = incidents.Count(i => i.ResolvedAt >= monthStart && i.ResolvedAt <= monthEnd),
                Closed = incidents.Count(i => i.Status == IncidentStatus.Closed && 
                    i.UpdatedAt >= monthStart && i.UpdatedAt <= monthEnd),
                Open = monthIncidents.Count(i => i.Status == IncidentStatus.Open || i.Status == IncidentStatus.InProgress)
            });
        }

        return trend;
    }

    // Helper methods para nombres y colores
    private string GetStatusDisplayName(IncidentStatus status) => status switch
    {
        IncidentStatus.Open => "Abierto",
        IncidentStatus.InProgress => "En Progreso",
        IncidentStatus.Escalated => "Escalado",
        IncidentStatus.Resolved => "Resuelto",
        IncidentStatus.Closed => "Cerrado",
        _ => status.ToString()
    };

    private string GetStatusColor(IncidentStatus status) => status switch
    {
        IncidentStatus.Open => "#3B82F6",      // blue-500
        IncidentStatus.InProgress => "#F59E0B", // amber-500
        IncidentStatus.Escalated => "#EF4444",  // red-500
        IncidentStatus.Resolved => "#10B981",   // emerald-500
        IncidentStatus.Closed => "#6B7280",     // gray-500
        _ => "#9CA3AF"
    };

    private string GetPriorityDisplayName(IncidentPriority priority) => priority switch
    {
        IncidentPriority.Low => "Baja",
        IncidentPriority.Medium => "Media",
        IncidentPriority.High => "Alta",
        IncidentPriority.Critical => "Crítica",
        _ => priority.ToString()
    };

    private string GetPriorityColor(IncidentPriority priority) => priority switch
    {
        IncidentPriority.Low => "#10B981",      // green-500
        IncidentPriority.Medium => "#F59E0B",   // amber-500
        IncidentPriority.High => "#F97316",     // orange-500
        IncidentPriority.Critical => "#EF4444", // red-500
        _ => "#9CA3AF"
    };

    private string GetTypeDisplayName(IncidentType type) => type switch
    {
        IncidentType.Failure => "Falla",
        IncidentType.Query => "Consulta",
        IncidentType.Request => "Requerimiento",
        _ => type.ToString()
    };

    private string GetTypeColor(IncidentType type) => type switch
    {
        IncidentType.Failure => "#EF4444",   // red-500
        IncidentType.Query => "#3B82F6",     // blue-500
        IncidentType.Request => "#8B5CF6",   // violet-500
        _ => "#9CA3AF"
    };
}
