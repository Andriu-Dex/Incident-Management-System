using IncidentsTI.Application.DTOs.Statistics;
using MediatR;

namespace IncidentsTI.Application.Queries;

/// <summary>
/// Query para obtener datos de tendencias para gráficos
/// </summary>
public class GetTrendDataQuery : IRequest<List<TrendDataDto>>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    /// <summary>
    /// Tipo de agrupación: "daily", "weekly", "monthly"
    /// </summary>
    public string GroupBy { get; set; } = "daily";
}
