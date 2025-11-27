using IncidentsTI.Application.Commands;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

/// <summary>
/// Handler para vincular un artículo de conocimiento a un incidente
/// </summary>
public class LinkArticleToIncidentCommandHandler : IRequestHandler<LinkArticleToIncidentCommand, bool>
{
    private readonly IKnowledgeArticleRepository _articleRepository;
    private readonly IIncidentRepository _incidentRepository;
    private readonly INotificationService _notificationService;

    public LinkArticleToIncidentCommandHandler(
        IKnowledgeArticleRepository articleRepository,
        IIncidentRepository incidentRepository,
        INotificationService notificationService)
    {
        _articleRepository = articleRepository;
        _incidentRepository = incidentRepository;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(LinkArticleToIncidentCommand request, CancellationToken cancellationToken)
    {
        // Verificar que el artículo existe
        var article = await _articleRepository.GetByIdAsync(request.ArticleId);
        if (article == null)
            return false;

        // Verificar que el incidente existe
        var incident = await _incidentRepository.GetByIdAsync(request.IncidentId);
        if (incident == null)
            return false;

        // Crear el vínculo
        var link = new IncidentArticleLink
        {
            IncidentId = request.IncidentId,
            ArticleId = request.ArticleId,
            LinkedByUserId = request.LinkedByUserId,
            LinkedAt = DateTime.UtcNow,
            WasHelpful = request.WasHelpful,
            Notes = request.Notes
        };

        await _articleRepository.AddLinkAsync(link);
        
        // Incrementar contador de uso del artículo
        await _articleRepository.IncrementUsageCountAsync(request.ArticleId);
        
        // Enviar notificación al usuario que reportó el incidente
        await _notificationService.NotifyArticleLinkedAsync(incident, article);

        return true;
    }
}
