namespace IncidentsTI.Application.DTOs.Escalation;

public class EscalationLevelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsActive { get; set; }
}
