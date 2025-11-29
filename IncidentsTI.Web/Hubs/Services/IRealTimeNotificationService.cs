namespace IncidentsTI.Web.Hubs.Services;

/// <summary>
/// Interfaz para comunicación en tiempo real via SignalR
/// Soporta notificaciones, actualizaciones de datos y eventos generales
/// </summary>
public interface IRealTimeNotificationService
{
    #region Notificaciones Toast
    
    /// <summary>
    /// Envía una notificación toast a un usuario específico
    /// </summary>
    Task NotifyUserAsync(string userId, string title, string message, string? url = null);
    
    /// <summary>
    /// Envía una notificación toast a todos los usuarios conectados
    /// </summary>
    Task NotifyAllAsync(string title, string message, string? url = null);
    
    /// <summary>
    /// Envía una notificación toast a un grupo específico (ej: "Admins", "Technicians")
    /// </summary>
    Task NotifyGroupAsync(string groupName, string title, string message, string? url = null);
    
    #endregion
    
    #region Actualizaciones de Datos
    
    /// <summary>
    /// Notifica que un incidente ha sido creado/actualizado (para refrescar listas)
    /// </summary>
    Task SendIncidentUpdateAsync(int incidentId, string action);
    
    /// <summary>
    /// Notifica actualización de dashboard (contadores, métricas)
    /// </summary>
    Task SendDashboardRefreshAsync(string targetGroup);
    
    /// <summary>
    /// Notifica que el contador de notificaciones cambió para un usuario
    /// </summary>
    Task SendNotificationCountUpdateAsync(string userId, int count);
    
    #endregion
    
    #region Eventos Genéricos
    
    /// <summary>
    /// Envía un evento genérico a un grupo
    /// </summary>
    Task SendEventToGroupAsync(string groupName, string eventName, object data);
    
    /// <summary>
    /// Envía un evento genérico a un usuario
    /// </summary>
    Task SendEventToUserAsync(string userId, string eventName, object data);
    
    /// <summary>
    /// Envía un evento genérico a todos
    /// </summary>
    Task SendEventToAllAsync(string eventName, object data);
    
    #endregion
}
