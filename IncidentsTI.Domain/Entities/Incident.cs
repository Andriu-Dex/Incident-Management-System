using IncidentsTI.Domain.Enums;

namespace IncidentsTI.Domain.Entities;

public class Incident
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    
    // Usuario que creó el incidente
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    
    // Servicio asociado
    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;
    
    // Información del incidente
    public IncidentType Type { get; set; }
    public IncidentPriority Priority { get; set; }
    public IncidentStatus Status { get; set; } = IncidentStatus.Open;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Técnico asignado (nullable)
    public string? AssignedToId { get; set; }
    public ApplicationUser? AssignedTo { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Colecciones de navegación para trazabilidad
    public ICollection<IncidentHistory> History { get; set; } = new List<IncidentHistory>();
    public ICollection<IncidentComment> Comments { get; set; } = new List<IncidentComment>();
}
