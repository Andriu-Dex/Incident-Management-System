using IncidentsTI.Domain.Enums;

namespace IncidentsTI.Application.DTOs;

public class IncidentDto
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    
    // Usuario creador
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    
    // Servicio
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string ServiceCategory { get; set; } = string.Empty;
    
    // Información del incidente
    public IncidentType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public IncidentPriority Priority { get; set; }
    public string PriorityName { get; set; } = string.Empty;
    public IncidentStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Técnico asignado
    public string? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    public string? AssignedToEmail { get; set; }
    
    // Escalamiento
    public int? CurrentEscalationLevelId { get; set; }
    public string? CurrentEscalationLevelName { get; set; }
    public int? CurrentEscalationLevelOrder { get; set; }
    
    // Datos de resolución
    public string? ResolutionDescription { get; set; }
    public string? RootCause { get; set; }
    public int? ResolutionTimeMinutes { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedById { get; set; }
    public string? ResolvedByName { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
