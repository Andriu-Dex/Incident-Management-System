using IncidentsTI.Application.Commands;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

/// <summary>
/// Handler para eliminar permanentemente un artículo de conocimiento
/// </summary>
public class DeleteKnowledgeArticleCommandHandler : IRequestHandler<DeleteKnowledgeArticleCommand, bool>
{
    private readonly IKnowledgeArticleRepository _articleRepository;

    public DeleteKnowledgeArticleCommandHandler(IKnowledgeArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<bool> Handle(DeleteKnowledgeArticleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var article = await _articleRepository.GetByIdAsync(request.ArticleId);
            if (article == null)
            {
                return false;
            }

            await _articleRepository.DeleteAsync(article);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
