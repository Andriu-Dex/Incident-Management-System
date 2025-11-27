using IncidentsTI.Application.DTOs.Knowledge;
using IncidentsTI.Application.Queries;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

/// <summary>
/// Handler para obtener los vínculos de artículos de un incidente
/// </summary>
public class GetIncidentArticleLinksQueryHandler : IRequestHandler<GetIncidentArticleLinksQuery, IEnumerable<IncidentArticleLinkDto>>
{
    private readonly IKnowledgeArticleRepository _articleRepository;
    private readonly IUserRepository _userRepository;

    public GetIncidentArticleLinksQueryHandler(
        IKnowledgeArticleRepository articleRepository,
        IUserRepository userRepository)
    {
        _articleRepository = articleRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<IncidentArticleLinkDto>> Handle(GetIncidentArticleLinksQuery request, CancellationToken cancellationToken)
    {
        var links = await _articleRepository.GetIncidentLinksAsync(request.IncidentId);
        
        var users = await _userRepository.GetAllAsync();
        var userDict = users.ToDictionary(u => u.Id, u => u);

        var result = new List<IncidentArticleLinkDto>();

        foreach (var link in links)
        {
            // Obtener detalles del artículo
            var article = await _articleRepository.GetByIdAsync(link.ArticleId);
            
            result.Add(new IncidentArticleLinkDto
            {
                Id = link.Id,
                IncidentId = link.IncidentId,
                ArticleId = link.ArticleId,
                ArticleTitle = article?.Title ?? "Artículo no encontrado",
                LinkedByUserName = userDict.TryGetValue(link.LinkedByUserId, out var user)
                    ? $"{user.FirstName} {user.LastName}"
                    : "Usuario desconocido",
                LinkedAt = link.LinkedAt,
                WasHelpful = link.WasHelpful,
                Notes = link.Notes
            });
        }

        return result.OrderByDescending(l => l.LinkedAt);
    }
}
