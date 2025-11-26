using IncidentsTI.Application.Commands;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

/// <summary>
/// Handler for deleting an incident
/// </summary>
public class DeleteIncidentCommandHandler : IRequestHandler<DeleteIncidentCommand, bool>
{
    private readonly IIncidentRepository _incidentRepository;

    public DeleteIncidentCommandHandler(IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<bool> Handle(DeleteIncidentCommand request, CancellationToken cancellationToken)
    {
        return await _incidentRepository.DeleteAsync(request.IncidentId);
    }
}
