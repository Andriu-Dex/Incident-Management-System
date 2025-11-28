using IncidentsTI.Application.DTOs.Statistics;
using MediatR;

namespace IncidentsTI.Application.Queries;

/// <summary>
/// Query para obtener todas las estadísticas del dashboard
/// </summary>
public class GetDashboardStatisticsQuery : IRequest<DashboardStatisticsDto>
{
    /// <summary>
    /// Fecha de inicio del período de análisis (opcional, por defecto últimos 30 días)
    /// </summary>
    public DateTime? StartDate { get; set; }
    
    /// <summary>
    /// Fecha de fin del período de análisis (opcional, por defecto hoy)
    /// </summary>
    public DateTime? EndDate { get; set; }
}
