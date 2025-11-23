using MediatR;

namespace IncidentsTI.Application.Commands;

public class UpdateIncidentStatusCommand : IRequest<bool>
{
    public int IncidentId { get; set; }
    public IncidentsTI.Domain.Enums.IncidentStatus NewStatus { get; set; }
}
