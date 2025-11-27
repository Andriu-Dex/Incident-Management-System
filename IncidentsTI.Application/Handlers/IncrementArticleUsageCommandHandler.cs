using IncidentsTI.Application.Commands;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

/// <summary>
/// Handler para incrementar el contador de uso de un artículo
/// </summary>
public class IncrementArticleUsageCommandHandler : IRequestHandler<IncrementArticleUsageCommand, bool>
{
    private readonly IKnowledgeArticleRepository _articleRepository;

    public IncrementArticleUsageCommandHandler(IKnowledgeArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<bool> Handle(IncrementArticleUsageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _articleRepository.IncrementUsageCountAsync(request.ArticleId);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
