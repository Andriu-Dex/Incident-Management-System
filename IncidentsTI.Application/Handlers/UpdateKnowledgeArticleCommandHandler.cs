using IncidentsTI.Application.Commands;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

/// <summary>
/// Handler para actualizar un artículo de conocimiento existente
/// </summary>
public class UpdateKnowledgeArticleCommandHandler : IRequestHandler<UpdateKnowledgeArticleCommand, bool>
{
    private readonly IKnowledgeArticleRepository _articleRepository;

    public UpdateKnowledgeArticleCommandHandler(IKnowledgeArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<bool> Handle(UpdateKnowledgeArticleCommand request, CancellationToken cancellationToken)
    {
        var article = await _articleRepository.GetByIdWithDetailsAsync(request.Id);
        if (article == null)
            return false;

        // Actualizar propiedades básicas
        article.Title = request.Title;
        article.ServiceId = request.ServiceId;
        article.IncidentType = request.IncidentType;
        article.ProblemDescription = request.ProblemDescription;
        article.Recommendations = request.Recommendations;
        article.EstimatedResolutionTimeMinutes = request.EstimatedResolutionTimeMinutes;
        article.UpdatedAt = DateTime.UtcNow;

        // Reemplazar pasos (eliminar existentes y agregar nuevos)
        article.Steps.Clear();
        foreach (var step in request.Steps.OrderBy(s => s.StepNumber))
        {
            article.Steps.Add(new SolutionStep
            {
                ArticleId = article.Id,
                StepNumber = step.StepNumber,
                Title = step.Title,
                Description = step.Description,
                Note = step.Note
            });
        }

        // Reemplazar keywords
        article.Keywords.Clear();
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
                    ArticleId = article.Id,
                    Keyword = keyword
                });
            }
        }

        await _articleRepository.UpdateAsync(article);
        return true;
    }
}
