using IncidentsTI.Domain.Enums;

namespace IncidentsTI.Domain.Entities;

/// <summary>
/// Registro de historial de cambios de un incidente para trazabilidad completa
/// </summary>
public class IncidentHistory
{
    public int Id { get; set; }
    
    /// <summary>
    /// Incidente al que pertenece este registro
    /// </summary>
    public int IncidentId { get; set; }
    public Incident Incident { get; set; } = null!;
    
    /// <summary>
    /// Usuario que realizó la acción
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    
    /// <summary>
    /// Tipo de acción realizada
    /// </summary>
    public HistoryAction Action { get; set; }
    
    /// <summary>
    /// Valor anterior (puede ser null para acciones de creación)
    /// </summary>
    public string? OldValue { get; set; }
    
    /// <summary>
    /// Nuevo valor
    /// </summary>
    public string? NewValue { get; set; }
    
    /// <summary>
    /// Descripción adicional de la acción (opcional)
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Fecha y hora de la acción
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
