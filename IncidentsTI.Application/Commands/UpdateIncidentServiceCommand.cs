using MediatR;

namespace IncidentsTI.Application.Commands;

public class UpdateIncidentServiceCommand : IRequest<bool>
{
    public int IncidentId { get; set; }
    public int NewServiceId { get; set; }
}
