using Microsoft.AspNetCore.SignalR;

namespace IncidentsTI.Web.Hubs.Services;

/// <summary>
/// Implementación del servicio de comunicación en tiempo real
/// </summary>
public class RealTimeNotificationService : IRealTimeNotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<RealTimeNotificationService> _logger;

    public RealTimeNotificationService(
        IHubContext<NotificationHub> hubContext,
        ILogger<RealTimeNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    #region Notificaciones Toast

    public async Task NotifyUserAsync(string userId, string title, string message, string? url = null)
    {
        try
        {
            var notification = new { Title = title, Message = message, Url = url, Timestamp = DateTime.UtcNow };
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", notification);
            _logger.LogDebug("Notificación enviada al usuario {UserId}: {Title}", userId, title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificación al usuario {UserId}", userId);
        }
    }

    public async Task NotifyAllAsync(string title, string message, string? url = null)
    {
        try
        {
            var notification = new { Title = title, Message = message, Url = url, Timestamp = DateTime.UtcNow };
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
            _logger.LogDebug("Notificación enviada a todos: {Title}", title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificación a todos");
        }
    }

    public async Task NotifyGroupAsync(string groupName, string title, string message, string? url = null)
    {
        try
        {
            var notification = new { Title = title, Message = message, Url = url, Timestamp = DateTime.UtcNow };
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", notification);
            _logger.LogDebug("Notificación enviada al grupo {GroupName}: {Title}", groupName, title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificación al grupo {GroupName}", groupName);
        }
    }

    #endregion

    #region Actualizaciones de Datos

    public async Task SendIncidentUpdateAsync(int incidentId, string action)
    {
        try
        {
            var data = new { IncidentId = incidentId, Action = action, Timestamp = DateTime.UtcNow };
            
            // Notificar a técnicos y admins que deben refrescar
            await _hubContext.Clients.Group("Technicians").SendAsync("IncidentUpdated", data);
            await _hubContext.Clients.Group("Admins").SendAsync("IncidentUpdated", data);
            
            _logger.LogDebug("Actualización de incidente {IncidentId}: {Action}", incidentId, action);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar actualización de incidente {IncidentId}", incidentId);
        }
    }

    public async Task SendDashboardRefreshAsync(string targetGroup)
    {
        try
        {
            var data = new { Timestamp = DateTime.UtcNow };
            await _hubContext.Clients.Group(targetGroup).SendAsync("DashboardRefresh", data);
            _logger.LogDebug("Refresh de dashboard enviado a {Group}", targetGroup);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar refresh de dashboard a {Group}", targetGroup);
        }
    }

    public async Task SendNotificationCountUpdateAsync(string userId, int count)
    {
        try
        {
            await _hubContext.Clients.User(userId).SendAsync("NotificationCountUpdated", count);
            _logger.LogDebug("Contador de notificaciones actualizado para {UserId}: {Count}", userId, count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar contador de notificaciones a {UserId}", userId);
        }
    }

    #endregion

    #region Eventos Genéricos

    public async Task SendEventToGroupAsync(string groupName, string eventName, object data)
    {
        try
        {
            await _hubContext.Clients.Group(groupName).SendAsync(eventName, data);
            _logger.LogDebug("Evento {EventName} enviado al grupo {Group}", eventName, groupName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar evento {EventName} al grupo {Group}", eventName, groupName);
        }
    }

    public async Task SendEventToUserAsync(string userId, string eventName, object data)
    {
        try
        {
            await _hubContext.Clients.User(userId).SendAsync(eventName, data);
            _logger.LogDebug("Evento {EventName} enviado al usuario {UserId}", eventName, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar evento {EventName} al usuario {UserId}", eventName, userId);
        }
    }

    public async Task SendEventToAllAsync(string eventName, object data)
    {
        try
        {
            await _hubContext.Clients.All.SendAsync(eventName, data);
            _logger.LogDebug("Evento {EventName} enviado a todos", eventName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar evento {EventName} a todos", eventName);
        }
    }

    #endregion
}
