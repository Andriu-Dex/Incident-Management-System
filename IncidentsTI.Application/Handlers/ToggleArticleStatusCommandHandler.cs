using IncidentsTI.Application.Commands;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

/// <summary>
/// Handler para activar/desactivar un artículo de conocimiento
/// </summary>
public class ToggleArticleStatusCommandHandler : IRequestHandler<ToggleArticleStatusCommand, bool>
{
    private readonly IKnowledgeArticleRepository _articleRepository;

    public ToggleArticleStatusCommandHandler(IKnowledgeArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<bool> Handle(ToggleArticleStatusCommand request, CancellationToken cancellationToken)
    {
        var article = await _articleRepository.GetByIdAsync(request.ArticleId);
        if (article == null)
            return false;

        article.IsActive = !article.IsActive;
        article.UpdatedAt = DateTime.UtcNow;
        
        await _articleRepository.UpdateAsync(article);
        return true;
    }
}
