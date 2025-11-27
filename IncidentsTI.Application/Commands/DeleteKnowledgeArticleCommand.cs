using MediatR;

namespace IncidentsTI.Application.Commands;

/// <summary>
/// Comando para eliminar permanentemente un artículo de conocimiento
/// </summary>
public class DeleteKnowledgeArticleCommand : IRequest<bool>
{
    public int ArticleId { get; set; }
}
