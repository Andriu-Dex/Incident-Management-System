using MediatR;

namespace IncidentsTI.Application.Commands;

/// <summary>
/// Comando para vincular un artículo de conocimiento a un incidente
/// </summary>
public class LinkArticleToIncidentCommand : IRequest<bool>
{
    public int IncidentId { get; set; }
    public int ArticleId { get; set; }
    public string LinkedByUserId { get; set; } = string.Empty;
    public bool WasHelpful { get; set; } = true;
    public string? Notes { get; set; }
}
