using IncidentsTI.Domain.Enums;
using MediatR;

namespace IncidentsTI.Application.Commands;

public class UpdateIncidentPriorityCommand : IRequest<bool>
{
    public int IncidentId { get; set; }
    public IncidentPriority NewPriority { get; set; }
}
