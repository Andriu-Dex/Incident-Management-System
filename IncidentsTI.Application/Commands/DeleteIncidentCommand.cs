using MediatR;

namespace IncidentsTI.Application.Commands;

/// <summary>
/// Command for deleting an incident
/// </summary>
public class DeleteIncidentCommand : IRequest<bool>
{
    public int IncidentId { get; set; }
}
