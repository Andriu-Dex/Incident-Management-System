using IncidentsTI.Domain.Enums;

namespace IncidentsTI.Domain.Entities;

/// <summary>
/// Entidad que representa una notificación del sistema
/// </summary>
public class Notification
{
    public int Id { get; set; }
    
    /// <summary>
    /// ID del usuario destinatario de la notificación
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Usuario destinatario
    /// </summary>
    public ApplicationUser? User { get; set; }
    
    /// <summary>
    /// Título de la notificación
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Mensaje detallado de la notificación
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Tipo de notificación
    /// </summary>
    public NotificationType Type { get; set; }
    
    /// <summary>
    /// ID de la entidad relacionada (normalmente IncidentId)
    /// </summary>
    public int? RelatedEntityId { get; set; }
    
    /// <summary>
    /// Tipo de entidad relacionada para navegación
    /// </summary>
    public string? RelatedEntityType { get; set; }
    
    /// <summary>
    /// URL para navegar al ver la notificación
    /// </summary>
    public string? ActionUrl { get; set; }
    
    /// <summary>
    /// Indica si la notificación ha sido leída
    /// </summary>
    public bool IsRead { get; set; } = false;
    
    /// <summary>
    /// Fecha en que fue leída
    /// </summary>
    public DateTime? ReadAt { get; set; }
    
    /// <summary>
    /// Fecha de creación
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Indica si la notificación está activa (no eliminada)
    /// </summary>
    public bool IsActive { get; set; } = true;
}
