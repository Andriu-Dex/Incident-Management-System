namespace IncidentsTI.Domain.Entities;

/// <summary>
/// Represents an escalation level in the incident management system.
/// Levels define the hierarchy of support (e.g., Help Desk → Specialist → External Provider)
/// </summary>
public class EscalationLevel
{
    public int Id { get; set; }
    
    /// <summary>
    /// Name of the escalation level (e.g., "Nivel 1 - Mesa de Ayuda")
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Detailed description of this escalation level
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Order for sorting levels (1 = lowest, higher = more specialized)
    /// </summary>
    public int Order { get; set; }
    
    /// <summary>
    /// Whether this level is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Timestamp when this level was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Timestamp when this level was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<Incident> Incidents { get; set; } = new List<Incident>();
    public ICollection<IncidentEscalation> EscalationsFrom { get; set; } = new List<IncidentEscalation>();
    public ICollection<IncidentEscalation> EscalationsTo { get; set; } = new List<IncidentEscalation>();
}
