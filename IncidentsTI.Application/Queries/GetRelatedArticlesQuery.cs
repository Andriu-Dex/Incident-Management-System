using IncidentsTI.Application.DTOs.Knowledge;
using IncidentsTI.Domain.Enums;
using MediatR;

namespace IncidentsTI.Application.Queries;

/// <summary>
/// Query para obtener artículos relacionados (sugerencias)
/// </summary>
public class GetRelatedArticlesQuery : IRequest<IEnumerable<KnowledgeArticleListDto>>
{
    /// <summary>
    /// Servicio del incidente
    /// </summary>
    public int ServiceId { get; set; }
    
    /// <summary>
    /// Tipo de incidente
    /// </summary>
    public IncidentType IncidentType { get; set; }
    
    /// <summary>
    /// Artículo a excluir (cuando se ve uno específico)
    /// </summary>
    public int ExcludeArticleId { get; set; } = 0;
    
    /// <summary>
    /// Cantidad máxima de resultados
    /// </summary>
    public int MaxResults { get; set; } = 5;
}
