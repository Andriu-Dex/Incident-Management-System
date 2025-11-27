using IncidentsTI.Application.DTOs.Knowledge;
using IncidentsTI.Application.Queries;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

/// <summary>
/// Handler para obtener artículos relacionados a un servicio y tipo de incidente
/// </summary>
public class GetRelatedArticlesQueryHandler : IRequestHandler<GetRelatedArticlesQuery, IEnumerable<KnowledgeArticleListDto>>
{
    private readonly IKnowledgeArticleRepository _articleRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IUserRepository _userRepository;

    public GetRelatedArticlesQueryHandler(
        IKnowledgeArticleRepository articleRepository,
        IServiceRepository serviceRepository,
        IUserRepository userRepository)
    {
        _articleRepository = articleRepository;
        _serviceRepository = serviceRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<KnowledgeArticleListDto>> Handle(GetRelatedArticlesQuery request, CancellationToken cancellationToken)
    {
        var articles = await _articleRepository.GetRelatedArticlesAsync(
            request.ServiceId,
            request.IncidentType,
            request.ExcludeArticleId
        );

        // Solo artículos activos
        articles = articles.Where(a => a.IsActive);

        // Limitar cantidad
        if (request.MaxResults > 0)
        {
            articles = articles.Take(request.MaxResults);
        }

        var services = await _serviceRepository.GetAllAsync();
        var serviceDict = services.ToDictionary(s => s.Id, s => s);

        var users = await _userRepository.GetAllAsync();
        var userDict = users.ToDictionary(u => u.Id, u => u);

        return articles.Select(article => new KnowledgeArticleListDto
        {
            Id = article.Id,
            Title = article.Title,
            ServiceName = serviceDict.TryGetValue(article.ServiceId, out var service) 
                ? service.Name 
                : "Servicio desconocido",
            ServiceCategory = serviceDict.TryGetValue(article.ServiceId, out var svc) 
                ? svc.Category.ToString() 
                : "Sin categoría",
            IncidentType = article.IncidentType,
            IncidentTypeName = article.IncidentType.ToString(),
            ProblemDescription = article.ProblemDescription.Length > 150 
                ? article.ProblemDescription.Substring(0, 150) + "..." 
                : article.ProblemDescription,
            EstimatedResolutionTimeMinutes = article.EstimatedResolutionTimeMinutes,
            AuthorName = userDict.TryGetValue(article.AuthorId, out var author)
                ? $"{author.FirstName} {author.LastName}"
                : "Usuario desconocido",
            UsageCount = article.UsageCount,
            IsActive = article.IsActive,
            CreatedAt = article.CreatedAt,
            Keywords = article.Keywords.Select(k => k.Keyword).ToList()
        }).ToList();
    }
}
