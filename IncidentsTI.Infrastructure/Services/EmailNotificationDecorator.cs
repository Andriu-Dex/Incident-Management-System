using IncidentsTI.Application.Services;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Enums;
using IncidentsTI.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IncidentsTI.Infrastructure.Services;

/// <summary>
/// Decorador que agrega funcionalidad de envío de emails al servicio de notificaciones base
/// </summary>
public class EmailNotificationDecorator : INotificationService
{
    private readonly INotificationService _innerService;
    private readonly IEmailService _emailService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IIncidentRepository _incidentRepository;
    private readonly ILogger<EmailNotificationDecorator> _logger;

    public EmailNotificationDecorator(
        INotificationService innerService,
        IEmailService emailService,
        UserManager<ApplicationUser> userManager,
        IIncidentRepository incidentRepository,
        ILogger<EmailNotificationDecorator> logger)
    {
        _innerService = innerService;
        _emailService = emailService;
        _userManager = userManager;
        _incidentRepository = incidentRepository;
        _logger = logger;
    }

    public async Task NotifyIncidentCreatedAsync(Incident incident)
    {
        // Primero ejecutar la notificación in-app
        await _innerService.NotifyIncidentCreatedAsync(incident);

        // Luego enviar email al creador
        try
        {
            var user = await _userManager.FindByIdAsync(incident.UserId);
            if (user != null)
            {
                await _emailService.SendIncidentCreatedEmailAsync(incident, user);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending incident created email for incident {IncidentId}", incident.Id);
        }
    }

    public async Task NotifyStatusChangedAsync(Incident incident, IncidentStatus oldStatus, IncidentStatus newStatus, string changedByUserId)
    {
        await _innerService.NotifyStatusChangedAsync(incident, oldStatus, newStatus, changedByUserId);

        try
        {
            // Enviar email al creador si no es quien cambió el estado
            if (incident.UserId != changedByUserId)
            {
                var user = await _userManager.FindByIdAsync(incident.UserId);
                if (user != null)
                {
                    await _emailService.SendStatusChangedEmailAsync(incident, user, oldStatus, newStatus);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending status changed email for incident {IncidentId}", incident.Id);
        }
    }

    public async Task NotifyIncidentAssignedAsync(Incident incident, string assignedToUserId, string assignedByUserId)
    {
        await _innerService.NotifyIncidentAssignedAsync(incident, assignedToUserId, assignedByUserId);

        try
        {
            var assignedTo = await _userManager.FindByIdAsync(assignedToUserId);
            var assignedBy = await _userManager.FindByIdAsync(assignedByUserId);

            if (assignedTo != null && assignedBy != null)
            {
                await _emailService.SendIncidentAssignedEmailAsync(incident, assignedTo, assignedBy);
            }

            // También enviar email al creador del incidente
            var creator = await _userManager.FindByIdAsync(incident.UserId);
            if (creator != null && creator.Id != assignedByUserId && assignedBy != null)
            {
                await _emailService.SendStatusChangedEmailAsync(incident, creator, IncidentStatus.Open, IncidentStatus.InProgress);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending incident assigned email for incident {IncidentId}", incident.Id);
        }
    }

    public async Task NotifyIncidentReassignedAsync(Incident incident, string oldAssigneeId, string newAssigneeId, string reassignedByUserId)
    {
        await _innerService.NotifyIncidentReassignedAsync(incident, oldAssigneeId, newAssigneeId, reassignedByUserId);

        try
        {
            var newAssignee = await _userManager.FindByIdAsync(newAssigneeId);
            var reassignedBy = await _userManager.FindByIdAsync(reassignedByUserId);

            if (newAssignee != null && reassignedBy != null)
            {
                await _emailService.SendIncidentAssignedEmailAsync(incident, newAssignee, reassignedBy);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending incident reassigned email for incident {IncidentId}", incident.Id);
        }
    }

    public async Task NotifyIncidentEscalatedAsync(Incident incident, IncidentEscalation escalation)
    {
        await _innerService.NotifyIncidentEscalatedAsync(incident, escalation);

        try
        {
            var user = await _userManager.FindByIdAsync(incident.UserId);
            if (user != null)
            {
                var fromLevel = escalation.FromLevel?.Name ?? "Nivel inicial";
                var toLevel = escalation.ToLevel?.Name ?? "Nivel superior";
                await _emailService.SendIncidentEscalatedEmailAsync(incident, user, fromLevel, toLevel, escalation.Reason);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending incident escalated email for incident {IncidentId}", incident.Id);
        }
    }

    public async Task NotifyIncidentResolvedAsync(Incident incident, string resolvedByUserId)
    {
        await _innerService.NotifyIncidentResolvedAsync(incident, resolvedByUserId);

        try
        {
            var user = await _userManager.FindByIdAsync(incident.UserId);
            if (user != null)
            {
                await _emailService.SendIncidentResolvedEmailAsync(incident, user, incident.ResolutionDescription);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending incident resolved email for incident {IncidentId}", incident.Id);
        }
    }

    public async Task NotifyIncidentClosedAsync(Incident incident)
    {
        await _innerService.NotifyIncidentClosedAsync(incident);

        try
        {
            var user = await _userManager.FindByIdAsync(incident.UserId);
            if (user != null)
            {
                await _emailService.SendIncidentClosedEmailAsync(incident, user);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending incident closed email for incident {IncidentId}", incident.Id);
        }
    }

    public async Task NotifyCommentAddedAsync(Incident incident, IncidentComment comment)
    {
        await _innerService.NotifyCommentAddedAsync(incident, comment);

        try
        {
            var commenter = await _userManager.FindByIdAsync(comment.UserId);
            
            // Enviar email al creador del incidente si no es quien comentó
            if (incident.UserId != comment.UserId)
            {
                var creator = await _userManager.FindByIdAsync(incident.UserId);
                if (creator != null && commenter != null)
                {
                    await _emailService.SendCommentAddedEmailAsync(incident, creator, comment, commenter);
                }
            }

            // Enviar email al técnico asignado si no es quien comentó
            if (!string.IsNullOrEmpty(incident.AssignedToId) && incident.AssignedToId != comment.UserId)
            {
                var assignedTo = await _userManager.FindByIdAsync(incident.AssignedToId);
                if (assignedTo != null && commenter != null)
                {
                    await _emailService.SendCommentAddedEmailAsync(incident, assignedTo, comment, commenter);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending comment added email for incident {IncidentId}", incident.Id);
        }
    }

    public async Task NotifyArticleLinkedAsync(Incident incident, KnowledgeArticle article)
    {
        await _innerService.NotifyArticleLinkedAsync(incident, article);
        // No enviamos email para artículos vinculados (es una acción menor)
    }

    public async Task NotifyIncidentClaimedAsync(Incident incident, string claimedByUserId)
    {
        await _innerService.NotifyIncidentClaimedAsync(incident, claimedByUserId);

        try
        {
            var claimer = await _userManager.FindByIdAsync(claimedByUserId);
            var creator = await _userManager.FindByIdAsync(incident.UserId);

            // Enviar email al creador del incidente
            if (creator != null && claimer != null)
            {
                await _emailService.SendIncidentAssignedEmailAsync(incident, creator, claimer);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending incident claimed email for incident {IncidentId}", incident.Id);
        }
    }

    public Task SendNotificationAsync(string userId, string title, string message, NotificationType type, int? relatedEntityId = null, string? actionUrl = null)
    {
        return _innerService.SendNotificationAsync(userId, title, message, type, relatedEntityId, actionUrl);
    }

    public Task SendNotificationToManyAsync(IEnumerable<string> userIds, string title, string message, NotificationType type, int? relatedEntityId = null, string? actionUrl = null)
    {
        return _innerService.SendNotificationToManyAsync(userIds, title, message, type, relatedEntityId, actionUrl);
    }

    public Task SendNotificationToRoleAsync(string roleName, string title, string message, NotificationType type, int? relatedEntityId = null, string? actionUrl = null)
    {
        return _innerService.SendNotificationToRoleAsync(roleName, title, message, type, relatedEntityId, actionUrl);
    }
}
