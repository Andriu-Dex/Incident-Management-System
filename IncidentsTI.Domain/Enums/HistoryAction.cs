namespace IncidentsTI.Domain.Enums;

/// <summary>
/// Tipos de acciones que se registran en el historial de incidentes
/// </summary>
public enum HistoryAction
{
    /// <summary>
    /// Incidente creado
    /// </summary>
    Created = 0,
    
    /// <summary>
    /// Estado cambiado
    /// </summary>
    StatusChanged = 1,
    
    /// <summary>
    /// Prioridad cambiada
    /// </summary>
    PriorityChanged = 2,
    
    /// <summary>
    /// Tipo de incidente cambiado
    /// </summary>
    TypeChanged = 3,
    
    /// <summary>
    /// Servicio asociado cambiado
    /// </summary>
    ServiceChanged = 4,
    
    /// <summary>
    /// Incidente asignado a técnico
    /// </summary>
    Assigned = 5,
    
    /// <summary>
    /// Incidente reasignado a otro técnico
    /// </summary>
    Reassigned = 6,
    
    /// <summary>
    /// Comentario agregado
    /// </summary>
    CommentAdded = 7,
    
    /// <summary>
    /// Incidente escalado
    /// </summary>
    Escalated = 8,
    
    /// <summary>
    /// Título o descripción actualizados
    /// </summary>
    DetailsUpdated = 9,
    
    /// <summary>
    /// Incidente resuelto con documentación de solución
    /// </summary>
    Resolved = 10
}
