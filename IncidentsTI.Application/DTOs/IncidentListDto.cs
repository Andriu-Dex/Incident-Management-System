using IncidentsTI.Domain.Enums;

namespace IncidentsTI.Application.DTOs;

public class IncidentListDto
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public IncidentStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public IncidentPriority Priority { get; set; }
    public string PriorityName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? AssignedToName { get; set; }
    public DateTime CreatedAt { get; set; }
}
