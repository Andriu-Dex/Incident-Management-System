using IncidentsTI.Application.DTOs.Knowledge;
using IncidentsTI.Application.Queries;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

/// <summary>
/// Handler para obtener un artículo de conocimiento por su ID
/// </summary>
public class GetArticleByIdQueryHandler : IRequestHandler<GetArticleByIdQuery, KnowledgeArticleDto?>
{
    private readonly IKnowledgeArticleRepository _articleRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IUserRepository _userRepository;
    private readonly IIncidentRepository _incidentRepository;

    public GetArticleByIdQueryHandler(
        IKnowledgeArticleRepository articleRepository,
        IServiceRepository serviceRepository,
        IUserRepository userRepository,
        IIncidentRepository incidentRepository)
    {
        _articleRepository = articleRepository;
        _serviceRepository = serviceRepository;
        _userRepository = userRepository;
        _incidentRepository = incidentRepository;
    }

    public async Task<KnowledgeArticleDto?> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
    {
        var article = await _articleRepository.GetByIdWithDetailsAsync(request.Id);
        if (article == null)
            return null;

        // Obtener información relacionada
        var service = await _serviceRepository.GetByIdAsync(article.ServiceId);
        var author = await _userRepository.GetByIdAsync(article.AuthorId);
        
        string? originIncidentTicket = null;
        if (article.OriginIncidentId.HasValue)
        {
            var originIncident = await _incidentRepository.GetByIdAsync(article.OriginIncidentId.Value);
            originIncidentTicket = originIncident?.TicketNumber;
        }

        return new KnowledgeArticleDto
        {
            Id = article.Id,
            Title = article.Title,
            ServiceId = article.ServiceId,
            ServiceName = service?.Name ?? "Servicio desconocido",
            ServiceCategory = service?.Category.ToString() ?? "Sin categoría",
            IncidentType = article.IncidentType,
            IncidentTypeName = article.IncidentType.ToString(),
            ProblemDescription = article.ProblemDescription,
            Recommendations = article.Recommendations,
            EstimatedResolutionTimeMinutes = article.EstimatedResolutionTimeMinutes,
            AuthorId = article.AuthorId,
            AuthorName = author != null ? $"{author.FirstName} {author.LastName}" : "Usuario desconocido",
            OriginIncidentId = article.OriginIncidentId,
            OriginIncidentTicket = originIncidentTicket,
            UsageCount = article.UsageCount,
            IsActive = article.IsActive,
            CreatedAt = article.CreatedAt,
            UpdatedAt = article.UpdatedAt,
            Steps = article.Steps
                .OrderBy(s => s.StepNumber)
                .Select(s => new SolutionStepDto
                {
                    Id = s.Id,
                    StepNumber = s.StepNumber,
                    Title = s.Title,
                    Description = s.Description,
                    Note = s.Note
                }).ToList(),
            Keywords = article.Keywords.Select(k => k.Keyword).ToList()
        };
    }
}
