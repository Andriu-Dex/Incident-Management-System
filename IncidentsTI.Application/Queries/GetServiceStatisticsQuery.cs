using IncidentsTI.Application.DTOs.Statistics;
using MediatR;

namespace IncidentsTI.Application.Queries;

/// <summary>
/// Query para obtener estadísticas detalladas por servicio
/// </summary>
public class GetServiceStatisticsQuery : IRequest<List<ServiceStatDto>>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IncludeInactive { get; set; } = false;
}
