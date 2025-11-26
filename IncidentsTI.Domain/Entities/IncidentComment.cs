namespace IncidentsTI.Domain.Entities;

/// <summary>
/// Comentarios en un incidente (pueden ser públicos o internos para TI)
/// </summary>
public class IncidentComment
{
    public int Id { get; set; }
    
    /// <summary>
    /// Incidente al que pertenece este comentario
    /// </summary>
    public int IncidentId { get; set; }
    public Incident Incident { get; set; } = null!;
    
    /// <summary>
    /// Usuario que escribió el comentario
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    
    /// <summary>
    /// Contenido del comentario
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Si es true, el comentario solo es visible para personal de TI (técnicos y administradores)
    /// Si es false, el comentario es visible para todos incluyendo el usuario que creó el incidente
    /// </summary>
    public bool IsInternal { get; set; } = false;
    
    /// <summary>
    /// Fecha y hora de creación
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
