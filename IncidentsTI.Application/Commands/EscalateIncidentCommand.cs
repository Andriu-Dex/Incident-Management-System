using MediatR;

namespace IncidentsTI.Application.Commands;

public class EscalateIncidentCommand : IRequest<bool>
{
    public int IncidentId { get; set; }
    public int ToLevelId { get; set; }
    public string? ToUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string EscalatedByUserId { get; set; } = string.Empty;
}
