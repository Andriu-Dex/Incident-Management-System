using IncidentsTI.Application.DTOs.Knowledge;
using MediatR;

namespace IncidentsTI.Application.Queries;

/// <summary>
/// Query para obtener los vínculos de artículos de un incidente
/// </summary>
public class GetIncidentArticleLinksQuery : IRequest<IEnumerable<IncidentArticleLinkDto>>
{
    public int IncidentId { get; set; }
}
