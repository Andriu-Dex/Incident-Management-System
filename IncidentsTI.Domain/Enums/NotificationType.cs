namespace IncidentsTI.Domain.Enums;

/// <summary>
/// Tipos de notificaciones del sistema
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// Notificación cuando se crea un nuevo incidente
    /// </summary>
    IncidentCreated,
    
    /// <summary>
    /// Notificación cuando se cambia el estado de un incidente
    /// </summary>
    StatusChanged,
    
    /// <summary>
    /// Notificación cuando se asigna un incidente a un técnico
    /// </summary>
    IncidentAssigned,
    
    /// <summary>
    /// Notificación cuando se reasigna un incidente
    /// </summary>
    IncidentReassigned,
    
    /// <summary>
    /// Notificación cuando se escala un incidente
    /// </summary>
    IncidentEscalated,
    
    /// <summary>
    /// Notificación cuando se resuelve un incidente
    /// </summary>
    IncidentResolved,
    
    /// <summary>
    /// Notificación cuando se cierra un incidente
    /// </summary>
    IncidentClosed,
    
    /// <summary>
    /// Notificación cuando se agrega un comentario
    /// </summary>
    CommentAdded,
    
    /// <summary>
    /// Notificación cuando se vincula un artículo KB
    /// </summary>
    ArticleLinked,
    
    /// <summary>
    /// Notificación de recordatorio de SLA
    /// </summary>
    SlaWarning,
    
    /// <summary>
    /// Notificación general del sistema
    /// </summary>
    SystemMessage
}
