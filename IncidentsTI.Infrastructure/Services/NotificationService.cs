using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Enums;
using IncidentsTI.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IncidentsTI.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de notificaciones
/// </summary>
public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationRepository notificationRepository,
        UserManager<ApplicationUser> userManager,
        ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task NotifyIncidentCreatedAsync(Incident incident)
    {
        try
        {
            // Notificar al creador que su incidente fue recibido
            await SendNotificationAsync(
                incident.UserId,
                "Incidente Creado",
                $"Tu incidente '{incident.Title}' ha sido registrado con el número {incident.TicketNumber}.",
                NotificationType.IncidentCreated,
                incident.Id,
                $"/incidents/{incident.Id}");

            // Notificar a todos los técnicos y administradores
            await SendNotificationToRoleAsync(
                "Technician",
                "Nuevo Incidente",
                $"Se ha creado un nuevo incidente: {incident.TicketNumber} - {incident.Title}",
                NotificationType.IncidentCreated,
                incident.Id,
                $"/technician/dashboard");

            await SendNotificationToRoleAsync(
                "Administrator",
                "Nuevo Incidente",
                $"Se ha creado un nuevo incidente: {incident.TicketNumber} - {incident.Title}",
                NotificationType.IncidentCreated,
                incident.Id,
                $"/admin/incidents");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificaciones de incidente creado {IncidentId}", incident.Id);
        }
    }

    public async Task NotifyStatusChangedAsync(Incident incident, IncidentStatus oldStatus, IncidentStatus newStatus, string changedByUserId)
    {
        try
        {
            var statusText = GetStatusText(newStatus);
            
            // Notificar al creador del incidente
            if (incident.UserId != changedByUserId)
            {
                await SendNotificationAsync(
                    incident.UserId,
                    "Estado Actualizado",
                    $"El estado de tu incidente {incident.TicketNumber} cambió a: {statusText}",
                    NotificationType.StatusChanged,
                    incident.Id,
                    $"/incidents/{incident.Id}");
            }

            // Si está asignado, notificar al técnico asignado
            if (!string.IsNullOrEmpty(incident.AssignedToId) && incident.AssignedToId != changedByUserId)
            {
                await SendNotificationAsync(
                    incident.AssignedToId,
                    "Estado Actualizado",
                    $"El incidente {incident.TicketNumber} cambió a: {statusText}",
                    NotificationType.StatusChanged,
                    incident.Id,
                    $"/technician/dashboard");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificaciones de cambio de estado {IncidentId}", incident.Id);
        }
    }

    public async Task NotifyIncidentAssignedAsync(Incident incident, string assignedToUserId, string assignedByUserId)
    {
        try
        {
            // Notificar al técnico asignado
            await SendNotificationAsync(
                assignedToUserId,
                "Incidente Asignado",
                $"Se te ha asignado el incidente {incident.TicketNumber}: {incident.Title}",
                NotificationType.IncidentAssigned,
                incident.Id,
                $"/technician/dashboard");

            // Notificar al creador del incidente
            if (incident.UserId != assignedByUserId)
            {
                var techUser = await _userManager.FindByIdAsync(assignedToUserId);
                var techName = techUser != null ? $"{techUser.FirstName} {techUser.LastName}" : "un técnico";

                await SendNotificationAsync(
                    incident.UserId,
                    "Incidente en Progreso",
                    $"Tu incidente {incident.TicketNumber} ha sido asignado a {techName}.",
                    NotificationType.IncidentAssigned,
                    incident.Id,
                    $"/incidents/{incident.Id}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificaciones de asignación {IncidentId}", incident.Id);
        }
    }

    public async Task NotifyIncidentReassignedAsync(Incident incident, string oldAssigneeId, string newAssigneeId, string reassignedByUserId)
    {
        try
        {
            // Notificar al técnico anterior
            if (!string.IsNullOrEmpty(oldAssigneeId) && oldAssigneeId != reassignedByUserId)
            {
                await SendNotificationAsync(
                    oldAssigneeId,
                    "Incidente Reasignado",
                    $"El incidente {incident.TicketNumber} ha sido reasignado a otro técnico.",
                    NotificationType.IncidentReassigned,
                    incident.Id,
                    $"/technician/dashboard");
            }

            // Notificar al nuevo técnico asignado
            await SendNotificationAsync(
                newAssigneeId,
                "Incidente Asignado",
                $"Se te ha asignado el incidente {incident.TicketNumber}: {incident.Title}",
                NotificationType.IncidentAssigned,
                incident.Id,
                $"/technician/dashboard");

            // Notificar al creador
            if (incident.UserId != reassignedByUserId)
            {
                var techUser = await _userManager.FindByIdAsync(newAssigneeId);
                var techName = techUser != null ? $"{techUser.FirstName} {techUser.LastName}" : "un técnico";

                await SendNotificationAsync(
                    incident.UserId,
                    "Cambio de Técnico",
                    $"Tu incidente {incident.TicketNumber} ha sido reasignado a {techName}.",
                    NotificationType.IncidentReassigned,
                    incident.Id,
                    $"/incidents/{incident.Id}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificaciones de reasignación {IncidentId}", incident.Id);
        }
    }

    public async Task NotifyIncidentEscalatedAsync(Incident incident, IncidentEscalation escalation)
    {
        try
        {
            // Notificar al usuario que escaló (si es diferente al creador)
            if (escalation.FromUserId != incident.UserId)
            {
                await SendNotificationAsync(
                    incident.UserId,
                    "Incidente Escalado",
                    $"Tu incidente {incident.TicketNumber} ha sido escalado a nivel {escalation.ToLevel?.Name ?? "superior"}.",
                    NotificationType.IncidentEscalated,
                    incident.Id,
                    $"/incidents/{incident.Id}");
            }

            // Notificar al técnico asignado en el escalamiento
            if (!string.IsNullOrEmpty(escalation.ToUserId))
            {
                await SendNotificationAsync(
                    escalation.ToUserId,
                    "Escalamiento Recibido",
                    $"Se te ha escalado el incidente {incident.TicketNumber}: {incident.Title}",
                    NotificationType.IncidentEscalated,
                    incident.Id,
                    $"/technician/dashboard");
            }

            // Notificar a administradores sobre el escalamiento
            await SendNotificationToRoleAsync(
                "Administrator",
                "Incidente Escalado",
                $"El incidente {incident.TicketNumber} ha sido escalado a {escalation.ToLevel?.Name ?? "nivel superior"}.",
                NotificationType.IncidentEscalated,
                incident.Id,
                $"/admin/incidents");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificaciones de escalamiento {IncidentId}", incident.Id);
        }
    }

    public async Task NotifyIncidentResolvedAsync(Incident incident, string resolvedByUserId)
    {
        try
        {
            // Notificar al creador del incidente
            await SendNotificationAsync(
                incident.UserId,
                "Incidente Resuelto",
                $"Tu incidente {incident.TicketNumber} ha sido resuelto. Por favor, verifica la solución.",
                NotificationType.IncidentResolved,
                incident.Id,
                $"/incidents/{incident.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificación de resolución {IncidentId}", incident.Id);
        }
    }

    public async Task NotifyIncidentClosedAsync(Incident incident)
    {
        try
        {
            // Notificar al creador del incidente
            await SendNotificationAsync(
                incident.UserId,
                "Incidente Cerrado",
                $"Tu incidente {incident.TicketNumber} ha sido cerrado.",
                NotificationType.IncidentClosed,
                incident.Id,
                $"/incidents/{incident.Id}");

            // Si tiene técnico asignado, notificarle
            if (!string.IsNullOrEmpty(incident.AssignedToId))
            {
                await SendNotificationAsync(
                    incident.AssignedToId,
                    "Incidente Cerrado",
                    $"El incidente {incident.TicketNumber} ha sido cerrado.",
                    NotificationType.IncidentClosed,
                    incident.Id,
                    $"/technician/dashboard");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificación de cierre {IncidentId}", incident.Id);
        }
    }

    public async Task NotifyCommentAddedAsync(Incident incident, IncidentComment comment)
    {
        try
        {
            var usersToNotify = new HashSet<string>();
            
            // Agregar creador si no es quien comentó
            if (incident.UserId != comment.UserId)
            {
                usersToNotify.Add(incident.UserId);
            }

            // Agregar técnico asignado si no es quien comentó
            if (!string.IsNullOrEmpty(incident.AssignedToId) && incident.AssignedToId != comment.UserId)
            {
                usersToNotify.Add(incident.AssignedToId);
            }

            var commenterUser = await _userManager.FindByIdAsync(comment.UserId);
            var commenterName = commenterUser != null ? $"{commenterUser.FirstName} {commenterUser.LastName}" : "Alguien";

            await SendNotificationToManyAsync(
                usersToNotify,
                "Nuevo Comentario",
                $"{commenterName} comentó en el incidente {incident.TicketNumber}.",
                NotificationType.CommentAdded,
                incident.Id,
                $"/incidents/{incident.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificación de comentario {IncidentId}", incident.Id);
        }
    }

    public async Task NotifyArticleLinkedAsync(Incident incident, KnowledgeArticle article)
    {
        try
        {
            // Notificar al creador del incidente
            await SendNotificationAsync(
                incident.UserId,
                "Solución Vinculada",
                $"Se ha vinculado un artículo de conocimiento a tu incidente {incident.TicketNumber}: {article.Title}",
                NotificationType.ArticleLinked,
                incident.Id,
                $"/incidents/{incident.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificación de artículo vinculado {IncidentId}", incident.Id);
        }
    }

    public async Task SendNotificationAsync(string userId, string title, string message, NotificationType type, int? relatedEntityId = null, string? actionUrl = null)
    {
        try
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                RelatedEntityId = relatedEntityId,
                RelatedEntityType = relatedEntityId.HasValue ? "Incident" : null,
                ActionUrl = actionUrl
            };

            await _notificationRepository.CreateAsync(notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear notificación para usuario {UserId}", userId);
        }
    }

    public async Task SendNotificationToManyAsync(IEnumerable<string> userIds, string title, string message, NotificationType type, int? relatedEntityId = null, string? actionUrl = null)
    {
        try
        {
            var notifications = userIds.Select(userId => new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                RelatedEntityId = relatedEntityId,
                RelatedEntityType = relatedEntityId.HasValue ? "Incident" : null,
                ActionUrl = actionUrl
            });

            await _notificationRepository.CreateManyAsync(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear notificaciones para múltiples usuarios");
        }
    }

    public async Task SendNotificationToRoleAsync(string roleName, string title, string message, NotificationType type, int? relatedEntityId = null, string? actionUrl = null)
    {
        try
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            var userIds = usersInRole.Where(u => u.IsActive).Select(u => u.Id);

            await SendNotificationToManyAsync(userIds, title, message, type, relatedEntityId, actionUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear notificaciones para rol {RoleName}", roleName);
        }
    }

    private static string GetStatusText(IncidentStatus status) => status switch
    {
        IncidentStatus.Open => "Abierto",
        IncidentStatus.InProgress => "En Progreso",
        IncidentStatus.Escalated => "Escalado",
        IncidentStatus.Resolved => "Resuelto",
        IncidentStatus.Closed => "Cerrado",
        _ => status.ToString()
    };
}
