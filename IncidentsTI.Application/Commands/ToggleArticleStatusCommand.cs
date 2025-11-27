using MediatR;

namespace IncidentsTI.Application.Commands;

/// <summary>
/// Comando para activar/desactivar un artículo de conocimiento
/// </summary>
public class ToggleArticleStatusCommand : IRequest<bool>
{
    public int ArticleId { get; set; }
}
