using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Enums;
using IncidentsTI.Domain.Interfaces;
using IncidentsTI.Web.Hubs.Services;

namespace IncidentsTI.Web.Services;

/// <summary>
/// Decorador que agrega notificaciones en tiempo real al NotificationService existente
/// </summary>
public class RealTimeNotificationDecorator : INotificationService
{
    private readonly INotificationService _inner;
    private readonly IRealTimeNotificationService _realTimeService;
    private readonly ILogger<RealTimeNotificationDecorator> _logger;

    public RealTimeNotificationDecorator(
        INotificationService inner,
        IRealTimeNotificationService realTimeService,
        ILogger<RealTimeNotificationDecorator> logger)
    {
        _inner = inner;
        _realTimeService = realTimeService;
        _logger = logger;
    }

    public async Task NotifyIncidentCreatedAsync(Incident incident)
    {
        await _inner.NotifyIncidentCreatedAsync(incident);

        try
        {
            // Toast notifications
            await _realTimeService.NotifyGroupAsync(
                "Technicians",
                "🆕 Nuevo Incidente",
                $"{incident.TicketNumber} - {incident.Title}",
                "/technician/dashboard");

            await _realTimeService.NotifyGroupAsync(
                "Admins",
                "🆕 Nuevo Incidente",
                $"{incident.TicketNumber} - {incident.Title}",
                "/admin/incidents");

            // Actualización de datos (para refrescar listas/dashboards)
            await _realTimeService.SendIncidentUpdateAsync(incident.Id, "created");
            await _realTimeService.SendDashboardRefreshAsync("Technicians");
            await _realTimeService.SendDashboardRefreshAsync("Admins");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error en notificación tiempo real para incidente creado");
        }
    }

    public async Task NotifyStatusChangedAsync(Incident incident, IncidentStatus oldStatus, IncidentStatus newStatus, string changedByUserId)
    {
        await _inner.NotifyStatusChangedAsync(incident, oldStatus, newStatus, changedByUserId);

        try
        {
            await _realTimeService.NotifyUserAsync(
                incident.UserId,
                "📋 Estado Actualizado",
                $"{incident.TicketNumber}: {GetStatusName(newStatus)}",
                "/my-incidents");

            // Actualización de datos
            await _realTimeService.SendIncidentUpdateAsync(incident.Id, "status-changed");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error en notificación tiempo real para cambio de estado");
        }
    }

    public async Task NotifyIncidentAssignedAsync(Incident incident, string assignedToUserId, string assignedByUserId)
    {
        await _inner.NotifyIncidentAssignedAsync(incident, assignedToUserId, assignedByUserId);

        try
        {
            await _realTimeService.NotifyUserAsync(
                assignedToUserId,
                "👤 Incidente Asignado",
                $"{incident.TicketNumber} - {incident.Title}",
                "/technician/dashboard");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error en notificación tiempo real para asignación");
        }
    }

    public async Task NotifyIncidentReassignedAsync(Incident incident, string oldAssigneeId, string newAssigneeId, string reassignedByUserId)
    {
        await _inner.NotifyIncidentReassignedAsync(incident, oldAssigneeId, newAssigneeId, reassignedByUserId);

        try
        {
            await _realTimeService.NotifyUserAsync(
                newAssigneeId,
                "🔄 Incidente Reasignado",
                $"{incident.TicketNumber} - {incident.Title}",
                "/technician/dashboard");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error en notificación tiempo real para reasignación");
        }
    }

    public async Task NotifyIncidentEscalatedAsync(Incident incident, IncidentEscalation escalation)
    {
        await _inner.NotifyIncidentEscalatedAsync(incident, escalation);

        try
        {
            await _realTimeService.NotifyGroupAsync(
                "Admins",
                "⚠️ Incidente Escalado",
                $"{incident.TicketNumber} escalado",
                "/admin/incidents");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error en notificación tiempo real para escalación");
        }
    }

    public async Task NotifyIncidentResolvedAsync(Incident incident, string resolvedByUserId)
    {
        await _inner.NotifyIncidentResolvedAsync(incident, resolvedByUserId);

        try
        {
            await _realTimeService.NotifyUserAsync(
                incident.UserId,
                "✅ Incidente Resuelto",
                $"{incident.TicketNumber} ha sido resuelto",
                "/my-incidents");

            // Actualización de datos
            await _realTimeService.SendIncidentUpdateAsync(incident.Id, "resolved");
            await _realTimeService.SendDashboardRefreshAsync("Technicians");
            await _realTimeService.SendDashboardRefreshAsync("Admins");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error en notificación tiempo real para resolución");
        }
    }

    public async Task NotifyIncidentClosedAsync(Incident incident)
    {
        await _inner.NotifyIncidentClosedAsync(incident);

        try
        {
            await _realTimeService.NotifyUserAsync(
                incident.UserId,
                "🔒 Incidente Cerrado",
                $"{incident.TicketNumber} ha sido cerrado",
                "/my-incidents");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error en notificación tiempo real para cierre");
        }
    }

    public async Task NotifyCommentAddedAsync(Incident incident, IncidentComment comment)
    {
        await _inner.NotifyCommentAddedAsync(incident, comment);

        try
        {
            // Solo notificar si no es el propio usuario
            if (incident.UserId != comment.UserId)
            {
                await _realTimeService.NotifyUserAsync(
                    incident.UserId,
                    "💬 Nuevo Comentario",
                    $"Comentario en {incident.TicketNumber}",
                    "/my-incidents");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error en notificación tiempo real para comentario");
        }
    }

    public async Task NotifyArticleLinkedAsync(Incident incident, KnowledgeArticle article)
    {
        await _inner.NotifyArticleLinkedAsync(incident, article);
        // No requiere notificación en tiempo real
    }

    public async Task NotifyIncidentClaimedAsync(Incident incident, string claimedByUserId)
    {
        await _inner.NotifyIncidentClaimedAsync(incident, claimedByUserId);

        try
        {
            // Notificar al creador que alguien tomó su incidente
            await _realTimeService.NotifyUserAsync(
                incident.UserId,
                "👍 Incidente Reclamado",
                $"Alguien está trabajando en {incident.TicketNumber}",
                "/my-incidents");

            // Notificar a administradores
            await _realTimeService.NotifyGroupAsync(
                "Admins",
                "👤 Incidente Reclamado",
                $"{incident.TicketNumber} ha sido reclamado",
                "/admin/incidents");

            // Actualizar dashboards para que otros técnicos/pasantes vean que ya no está disponible
            await _realTimeService.SendIncidentUpdateAsync(incident.Id, "claimed");
            await _realTimeService.SendDashboardRefreshAsync("Technicians");
            await _realTimeService.SendDashboardRefreshAsync("Pasantes");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error en notificación tiempo real para incidente reclamado");
        }
    }

    public Task SendNotificationAsync(string userId, string title, string message, NotificationType type, int? relatedEntityId = null, string? actionUrl = null)
        => _inner.SendNotificationAsync(userId, title, message, type, relatedEntityId, actionUrl);

    public Task SendNotificationToManyAsync(IEnumerable<string> userIds, string title, string message, NotificationType type, int? relatedEntityId = null, string? actionUrl = null)
        => _inner.SendNotificationToManyAsync(userIds, title, message, type, relatedEntityId, actionUrl);

    public Task SendNotificationToRoleAsync(string roleName, string title, string message, NotificationType type, int? relatedEntityId = null, string? actionUrl = null)
        => _inner.SendNotificationToRoleAsync(roleName, title, message, type, relatedEntityId, actionUrl);

    private static string GetStatusName(IncidentStatus status) => status switch
    {
        IncidentStatus.Open => "Abierto",
        IncidentStatus.InProgress => "En Progreso",
        IncidentStatus.Escalated => "Escalado",
        IncidentStatus.Resolved => "Resuelto",
        IncidentStatus.Closed => "Cerrado",
        _ => status.ToString()
    };
}
