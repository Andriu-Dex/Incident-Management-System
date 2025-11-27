using IncidentsTI.Application.DTOs.Knowledge;
using IncidentsTI.Domain.Enums;
using MediatR;

namespace IncidentsTI.Application.Queries;

/// <summary>
/// Query para buscar artículos con filtros opcionales
/// </summary>
public class SearchKnowledgeArticlesQuery : IRequest<IEnumerable<KnowledgeArticleListDto>>
{
    /// <summary>
    /// Palabra clave para buscar en título, descripción y keywords
    /// </summary>
    public string? Keyword { get; set; }
    
    /// <summary>
    /// Filtrar por servicio
    /// </summary>
    public int? ServiceId { get; set; }
    
    /// <summary>
    /// Filtrar por tipo de incidente
    /// </summary>
    public IncidentType? IncidentType { get; set; }
    
    /// <summary>
    /// Incluir artículos inactivos (solo para admins)
    /// </summary>
    public bool IncludeInactive { get; set; } = false;
}
