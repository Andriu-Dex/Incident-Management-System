using MediatR;

namespace IncidentsTI.Application.Commands;

public class ResolveIncidentCommand : IRequest<bool>
{
    public int IncidentId { get; set; }
    public string UserId { get; set; } = string.Empty;
    
    // Opción 1: Vincular artículo existente
    public int? LinkedArticleId { get; set; }
    public bool ArticleWasHelpful { get; set; } = true;
    public string? LinkNotes { get; set; }
    
    // Opción 2: Documentar solución nueva
    public string? ResolutionDescription { get; set; }
    public string? RootCause { get; set; }
    public int? ResolutionTimeMinutes { get; set; }
}
