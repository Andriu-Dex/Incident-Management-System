using IncidentsTI.Application.DTOs.Statistics;
using MediatR;

namespace IncidentsTI.Application.Queries;

/// <summary>
/// Query para obtener estadísticas de rendimiento por técnico
/// </summary>
public class GetTechnicianStatisticsQuery : IRequest<List<TechnicianStatDto>>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
