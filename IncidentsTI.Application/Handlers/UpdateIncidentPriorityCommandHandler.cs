using IncidentsTI.Application.Commands;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

public class UpdateIncidentPriorityCommandHandler : IRequestHandler<UpdateIncidentPriorityCommand, bool>
{
    private readonly IIncidentRepository _incidentRepository;

    public UpdateIncidentPriorityCommandHandler(IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<bool> Handle(UpdateIncidentPriorityCommand request, CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetByIdAsync(request.IncidentId);
        
        if (incident == null)
            return false;

        incident.Priority = request.NewPriority;
        incident.UpdatedAt = DateTime.UtcNow;

        await _incidentRepository.UpdateAsync(incident);
        return true;
    }
}
