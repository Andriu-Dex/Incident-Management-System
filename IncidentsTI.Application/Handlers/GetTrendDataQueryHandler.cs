using IncidentsTI.Application.DTOs.Statistics;
using IncidentsTI.Application.Queries;
using IncidentsTI.Domain.Enums;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

public class GetTrendDataQueryHandler : IRequestHandler<GetTrendDataQuery, List<TrendDataDto>>
{
    private readonly IIncidentRepository _incidentRepository;

    public GetTrendDataQueryHandler(IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<List<TrendDataDto>> Handle(GetTrendDataQuery request, CancellationToken cancellationToken)
    {
        var endDate = request.EndDate ?? DateTime.UtcNow;
        var startDate = request.StartDate ?? endDate.AddDays(-30);

        var allIncidents = (await _incidentRepository.GetAllAsync()).ToList();

        return request.GroupBy.ToLower() switch
        {
            "weekly" => GetWeeklyTrend(allIncidents, startDate, endDate),
            "monthly" => GetMonthlyTrend(allIncidents, startDate, endDate),
            _ => GetDailyTrend(allIncidents, startDate, endDate)
        };
    }

    private List<TrendDataDto> GetDailyTrend(List<Domain.Entities.Incident> incidents, DateTime startDate, DateTime endDate)
    {
        var trend = new List<TrendDataDto>();
        var currentDate = startDate.Date;

        while (currentDate <= endDate.Date)
        {
            var dayIncidents = incidents.Where(i => i.CreatedAt.Date == currentDate).ToList();

            trend.Add(new TrendDataDto
            {
                Date = currentDate,
                Label = currentDate.ToString("dd/MM"),
                Created = dayIncidents.Count,
                Resolved = incidents.Count(i => i.ResolvedAt?.Date == currentDate),
                Closed = incidents.Count(i => i.Status == IncidentStatus.Closed && i.UpdatedAt?.Date == currentDate),
                Open = incidents.Count(i => 
                    i.CreatedAt.Date <= currentDate && 
                    (i.Status == IncidentStatus.Open || i.Status == IncidentStatus.InProgress))
            });

            currentDate = currentDate.AddDays(1);
        }

        return trend;
    }

    private List<TrendDataDto> GetWeeklyTrend(List<Domain.Entities.Incident> incidents, DateTime startDate, DateTime endDate)
    {
        var trend = new List<TrendDataDto>();
        
        // Ajustar al inicio de la semana (lunes)
        var currentWeekStart = startDate.Date.AddDays(-(int)startDate.DayOfWeek + (int)DayOfWeek.Monday);
        if (currentWeekStart > startDate.Date) currentWeekStart = currentWeekStart.AddDays(-7);

        while (currentWeekStart <= endDate.Date)
        {
            var weekEnd = currentWeekStart.AddDays(6);
            var weekIncidents = incidents.Where(i => 
                i.CreatedAt.Date >= currentWeekStart && 
                i.CreatedAt.Date <= weekEnd).ToList();

            trend.Add(new TrendDataDto
            {
                Date = currentWeekStart,
                Label = $"Sem {GetWeekNumber(currentWeekStart)}",
                Created = weekIncidents.Count,
                Resolved = incidents.Count(i => 
                    i.ResolvedAt?.Date >= currentWeekStart && 
                    i.ResolvedAt?.Date <= weekEnd),
                Closed = incidents.Count(i => 
                    i.Status == IncidentStatus.Closed && 
                    i.UpdatedAt?.Date >= currentWeekStart && 
                    i.UpdatedAt?.Date <= weekEnd),
                Open = incidents.Count(i => 
                    i.CreatedAt.Date <= weekEnd && 
                    (i.Status == IncidentStatus.Open || i.Status == IncidentStatus.InProgress))
            });

            currentWeekStart = currentWeekStart.AddDays(7);
        }

        return trend;
    }

    private List<TrendDataDto> GetMonthlyTrend(List<Domain.Entities.Incident> incidents, DateTime startDate, DateTime endDate)
    {
        var trend = new List<TrendDataDto>();
        var currentMonth = new DateTime(startDate.Year, startDate.Month, 1);

        while (currentMonth <= endDate)
        {
            var monthEnd = currentMonth.AddMonths(1).AddDays(-1);
            var monthIncidents = incidents.Where(i => 
                i.CreatedAt >= currentMonth && 
                i.CreatedAt <= monthEnd).ToList();

            trend.Add(new TrendDataDto
            {
                Date = currentMonth,
                Label = currentMonth.ToString("MMM yyyy"),
                Created = monthIncidents.Count,
                Resolved = incidents.Count(i => 
                    i.ResolvedAt >= currentMonth && 
                    i.ResolvedAt <= monthEnd),
                Closed = incidents.Count(i => 
                    i.Status == IncidentStatus.Closed && 
                    i.UpdatedAt >= currentMonth && 
                    i.UpdatedAt <= monthEnd),
                Open = monthIncidents.Count(i => 
                    i.Status == IncidentStatus.Open || 
                    i.Status == IncidentStatus.InProgress)
            });

            currentMonth = currentMonth.AddMonths(1);
        }

        return trend;
    }

    private int GetWeekNumber(DateTime date)
    {
        var cal = System.Globalization.CultureInfo.CurrentCulture.Calendar;
        return cal.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    }
}
