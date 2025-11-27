using IncidentsTI.Application.DTOs.Knowledge;
using IncidentsTI.Application.Queries;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

/// <summary>
/// Handler para buscar artículos de conocimiento con filtros
/// </summary>
public class SearchKnowledgeArticlesQueryHandler : IRequestHandler<SearchKnowledgeArticlesQuery, IEnumerable<KnowledgeArticleListDto>>
{
    private readonly IKnowledgeArticleRepository _articleRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IUserRepository _userRepository;

    public SearchKnowledgeArticlesQueryHandler(
        IKnowledgeArticleRepository articleRepository,
        IServiceRepository serviceRepository,
        IUserRepository userRepository)
    {
        _articleRepository = articleRepository;
        _serviceRepository = serviceRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<KnowledgeArticleListDto>> Handle(SearchKnowledgeArticlesQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.KnowledgeArticle> articles;
        
        if (request.IncludeInactive)
        {
            // Para admins: obtener todos y luego filtrar
            articles = await _articleRepository.SearchAsync(
                request.Keyword,
                request.ServiceId,
                request.IncidentType
            );
        }
        else
        {
            // Para usuarios normales: solo activos
            var allArticles = await _articleRepository.SearchAsync(
                request.Keyword,
                request.ServiceId,
                request.IncidentType
            );
            articles = allArticles.Where(a => a.IsActive);
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
            ProblemDescription = article.ProblemDescription.Length > 200 
                ? article.ProblemDescription.Substring(0, 200) + "..." 
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
