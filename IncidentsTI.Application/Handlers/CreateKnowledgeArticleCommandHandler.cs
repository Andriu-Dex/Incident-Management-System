using IncidentsTI.Application.Commands;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

/// <summary>
/// Handler para crear un nuevo artículo de conocimiento
/// </summary>
public class CreateKnowledgeArticleCommandHandler : IRequestHandler<CreateKnowledgeArticleCommand, int>
{
    private readonly IKnowledgeArticleRepository _articleRepository;

    public CreateKnowledgeArticleCommandHandler(IKnowledgeArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<int> Handle(CreateKnowledgeArticleCommand request, CancellationToken cancellationToken)
    {
        var article = new KnowledgeArticle
        {
            Title = request.Title,
            ServiceId = request.ServiceId,
            IncidentType = request.IncidentType,
            ProblemDescription = request.ProblemDescription,
            Recommendations = request.Recommendations,
            EstimatedResolutionTimeMinutes = request.EstimatedResolutionTimeMinutes,
            AuthorId = request.AuthorId,
            OriginIncidentId = request.OriginIncidentId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Agregar pasos
        foreach (var step in request.Steps.OrderBy(s => s.StepNumber))
        {
            article.Steps.Add(new SolutionStep
            {
                StepNumber = step.StepNumber,
                Title = step.Title,
                Description = step.Description,
                Note = step.Note
            });
        }

        // Agregar keywords
        if (!string.IsNullOrWhiteSpace(request.Keywords))
        {
            var keywords = request.Keywords
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(k => k.Trim().ToLowerInvariant())
                .Distinct();

            foreach (var keyword in keywords)
            {
                article.Keywords.Add(new ArticleKeyword
                {
                    Keyword = keyword
                });
            }
        }

        var created = await _articleRepository.AddAsync(article);
        return created.Id;
    }
}
