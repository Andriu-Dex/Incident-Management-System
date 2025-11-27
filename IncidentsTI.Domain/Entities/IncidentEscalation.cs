namespace IncidentsTI.Domain.Entities;

/// <summary>
/// Represents an escalation event for an incident.
/// Tracks when an incident was escalated, by whom, to whom, and why.
/// </summary>
public class IncidentEscalation
{
    public int Id { get; set; }
    
    /// <summary>
    /// The incident that was escalated
    /// </summary>
    public int IncidentId { get; set; }
    
    /// <summary>
    /// The user who performed the escalation
    /// </summary>
    public string FromUserId { get; set; } = string.Empty;
    
    /// <summary>
    /// The user to whom the incident was escalated (nullable if escalated to a level without specific user)
    /// </summary>
    public string? ToUserId { get; set; }
    
    /// <summary>
    /// The escalation level from which the incident was escalated (nullable if first escalation)
    /// </summary>
    public int? FromLevelId { get; set; }
    
    /// <summary>
    /// The escalation level to which the incident was escalated
    /// </summary>
    public int ToLevelId { get; set; }
    
    /// <summary>
    /// The reason for escalation
    /// </summary>
    public string Reason { get; set; } = string.Empty;
    
    /// <summary>
    /// Additional notes about the escalation
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Timestamp when the escalation occurred
    /// </summary>
    public DateTime EscalatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Incident Incident { get; set; } = null!;
    public ApplicationUser FromUser { get; set; } = null!;
    public ApplicationUser? ToUser { get; set; }
    public EscalationLevel? FromLevel { get; set; }
    public EscalationLevel ToLevel { get; set; } = null!;
    
    /// <summary>
    /// Gets a formatted description of the escalation for display
    /// </summary>
    public string FormattedDescription
    {
        get
        {
            var fromLevelName = FromLevel?.Name ?? "Sin nivel";
            var toLevelName = ToLevel?.Name ?? "Desconocido";
            var toUserName = ToUser != null ? $" a {ToUser.FirstName} {ToUser.LastName}" : "";
            
            return $"Escalado de '{fromLevelName}' a '{toLevelName}'{toUserName}";
        }
    }
}
