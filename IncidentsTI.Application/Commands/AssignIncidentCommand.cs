using MediatR;

namespace IncidentsTI.Application.Commands;

public class AssignIncidentCommand : IRequest<bool>
{
    public int IncidentId { get; set; }
    public string AssignedToId { get; set; } = string.Empty;
}
