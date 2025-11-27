using MediatR;

namespace IncidentsTI.Application.Commands;

/// <summary>
/// Comando para incrementar el contador de uso de un artículo de conocimiento
/// </summary>
public class IncrementArticleUsageCommand : IRequest<bool>
{
    public int ArticleId { get; set; }
}
