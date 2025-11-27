using IncidentsTI.Domain.Entities;

namespace IncidentsTI.Domain.Interfaces;

/// <summary>
/// Interfaz del repositorio de notificaciones
/// </summary>
public interface INotificationRepository
{
    /// <summary>
    /// Obtiene una notificación por su ID
    /// </summary>
    Task<Notification?> GetByIdAsync(int id);
    
    /// <summary>
    /// Obtiene todas las notificaciones de un usuario
    /// </summary>
    Task<IEnumerable<Notification>> GetByUserIdAsync(string userId, bool includeRead = true);
    
    /// <summary>
    /// Obtiene las notificaciones no leídas de un usuario
    /// </summary>
    Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(string userId);
    
    /// <summary>
    /// Obtiene el conteo de notificaciones no leídas de un usuario
    /// </summary>
    Task<int> GetUnreadCountAsync(string userId);
    
    /// <summary>
    /// Crea una nueva notificación
    /// </summary>
    Task<Notification> CreateAsync(Notification notification);
    
    /// <summary>
    /// Crea múltiples notificaciones
    /// </summary>
    Task<IEnumerable<Notification>> CreateManyAsync(IEnumerable<Notification> notifications);
    
    /// <summary>
    /// Marca una notificación como leída
    /// </summary>
    Task<bool> MarkAsReadAsync(int notificationId);
    
    /// <summary>
    /// Marca todas las notificaciones de un usuario como leídas
    /// </summary>
    Task<int> MarkAllAsReadAsync(string userId);
    
    /// <summary>
    /// Elimina (desactiva) una notificación
    /// </summary>
    Task<bool> DeleteAsync(int notificationId);
    
    /// <summary>
    /// Obtiene notificaciones paginadas de un usuario
    /// </summary>
    Task<(IEnumerable<Notification> Items, int TotalCount)> GetPaginatedAsync(
        string userId, 
        int page, 
        int pageSize, 
        bool includeRead = true);
}
