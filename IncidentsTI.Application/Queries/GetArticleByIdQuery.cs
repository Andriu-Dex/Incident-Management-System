using IncidentsTI.Application.DTOs.Knowledge;
using MediatR;

namespace IncidentsTI.Application.Queries;

/// <summary>
/// Query para obtener un artículo por ID con todos sus detalles
/// </summary>
public class GetArticleByIdQuery : IRequest<KnowledgeArticleDto?>
{
    public int Id { get; set; }
}
