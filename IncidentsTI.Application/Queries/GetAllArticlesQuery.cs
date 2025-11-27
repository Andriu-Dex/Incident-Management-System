using IncidentsTI.Application.DTOs.Knowledge;
using MediatR;

namespace IncidentsTI.Application.Queries;

/// <summary>
/// Query para obtener todos los artículos (para administración)
/// </summary>
public class GetAllArticlesQuery : IRequest<IEnumerable<KnowledgeArticleListDto>>
{
    /// <summary>
    /// Incluir artículos inactivos
    /// </summary>
    public bool IncludeInactive { get; set; } = false;
}
