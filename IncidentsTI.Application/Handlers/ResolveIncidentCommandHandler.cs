using IncidentsTI.Application.Commands;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Enums;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

/// <summary>
/// Handler para resolver un incidente con datos de resolución y opcionalmente vincular un artículo
/// </summary>
public class ResolveIncidentCommandHandler : IRequestHandler<ResolveIncidentCommand, bool>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IKnowledgeArticleRepository _articleRepository;

    public ResolveIncidentCommandHandler(
        IIncidentRepository incidentRepository,
        IKnowledgeArticleRepository articleRepository)
    {
        _incidentRepository = incidentRepository;
        _articleRepository = articleRepository;
    }

    public async Task<bool> Handle(ResolveIncidentCommand request, CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetByIdAsync(request.IncidentId);
        if (incident == null)
            return false;

        // Si se vincula un artículo, crear el link
        if (request.LinkedArticleId.HasValue)
        {
            var article = await _articleRepository.GetByIdAsync(request.LinkedArticleId.Value);
            if (article == null)
                return false;

            var link = new IncidentArticleLink
            {
                IncidentId = request.IncidentId,
                ArticleId = request.LinkedArticleId.Value,
                LinkedByUserId = request.UserId,
                LinkedAt = DateTime.UtcNow,
                WasHelpful = request.ArticleWasHelpful,
                Notes = request.LinkNotes
            };

            await _articleRepository.AddLinkAsync(link);
            await _articleRepository.IncrementUsageCountAsync(request.LinkedArticleId.Value);
        }

        // Actualizar datos de resolución del incidente
        incident.Status = IncidentStatus.Resolved;
        incident.ResolutionDescription = request.ResolutionDescription;
        incident.RootCause = request.RootCause;
        incident.ResolutionTimeMinutes = request.ResolutionTimeMinutes;
        incident.ResolvedAt = DateTime.UtcNow;
        incident.ResolvedById = request.UserId;
        incident.UpdatedAt = DateTime.UtcNow;

        // Agregar historial
        incident.History.Add(new IncidentHistory
        {
            IncidentId = request.IncidentId,
            UserId = request.UserId,
            Action = HistoryAction.Resolved,
            OldValue = incident.Status.ToString(),
            NewValue = IncidentStatus.Resolved.ToString(),
            Description = request.LinkedArticleId.HasValue 
                ? $"Resuelto con artículo KB vinculado. {request.LinkNotes ?? ""}"
                : $"Resuelto con solución documentada: {request.ResolutionDescription?.Substring(0, Math.Min(100, request.ResolutionDescription?.Length ?? 0))}...",
            Timestamp = DateTime.UtcNow
        });

        await _incidentRepository.UpdateAsync(incident);

        return true;
    }
}
