using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Enums;

namespace IncidentsTI.Domain.Interfaces;

/// <summary>
/// Interfaz del servicio de notificaciones
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Notifica cuando se crea un nuevo incidente
    /// </summary>
    Task NotifyIncidentCreatedAsync(Incident incident);
    
    /// <summary>
    /// Notifica cuando se cambia el estado de un incidente
    /// </summary>
    Task NotifyStatusChangedAsync(Incident incident, IncidentStatus oldStatus, IncidentStatus newStatus, string changedByUserId);
    
    /// <summary>
    /// Notifica cuando se asigna un incidente a un técnico
    /// </summary>
    Task NotifyIncidentAssignedAsync(Incident incident, string assignedToUserId, string assignedByUserId);
    
    /// <summary>
    /// Notifica cuando se reasigna un incidente
    /// </summary>
    Task NotifyIncidentReassignedAsync(Incident incident, string oldAssigneeId, string newAssigneeId, string reassignedByUserId);
    
    /// <summary>
    /// Notifica cuando se escala un incidente
    /// </summary>
    Task NotifyIncidentEscalatedAsync(Incident incident, IncidentEscalation escalation);
    
    /// <summary>
    /// Notifica cuando se resuelve un incidente
    /// </summary>
    Task NotifyIncidentResolvedAsync(Incident incident, string resolvedByUserId);
    
    /// <summary>
    /// Notifica cuando se cierra un incidente
    /// </summary>
    Task NotifyIncidentClosedAsync(Incident incident);
    
    /// <summary>
    /// Notifica cuando se agrega un comentario a un incidente
    /// </summary>
    Task NotifyCommentAddedAsync(Incident incident, IncidentComment comment);
    
    /// <summary>
    /// Notifica cuando se vincula un artículo KB a un incidente
    /// </summary>
    Task NotifyArticleLinkedAsync(Incident incident, KnowledgeArticle article);
    
    /// <summary>
    /// Envía una notificación personalizada a un usuario
    /// </summary>
    Task SendNotificationAsync(string userId, string title, string message, NotificationType type, int? relatedEntityId = null, string? actionUrl = null);
    
    /// <summary>
    /// Envía una notificación a múltiples usuarios
    /// </summary>
    Task SendNotificationToManyAsync(IEnumerable<string> userIds, string title, string message, NotificationType type, int? relatedEntityId = null, string? actionUrl = null);
    
    /// <summary>
    /// Envía notificación a todos los usuarios con un rol específico
    /// </summary>
    Task SendNotificationToRoleAsync(string roleName, string title, string message, NotificationType type, int? relatedEntityId = null, string? actionUrl = null);
}
