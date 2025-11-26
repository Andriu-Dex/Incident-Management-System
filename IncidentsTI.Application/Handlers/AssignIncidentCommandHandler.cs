using IncidentsTI.Application.Commands;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

public class AssignIncidentCommandHandler : IRequestHandler<AssignIncidentCommand, bool>
{
    private readonly IIncidentRepository _incidentRepository;

    public AssignIncidentCommandHandler(IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<bool> Handle(AssignIncidentCommand request, CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetByIdAsync(request.IncidentId);
        
        if (incident == null)
            return false;

        // Si AssignedToId está vacío, guardarlo como null para desasignar correctamente
        incident.AssignedToId = string.IsNullOrEmpty(request.AssignedToId) ? null : request.AssignedToId;
        incident.UpdatedAt = DateTime.UtcNow;

        await _incidentRepository.UpdateAsync(incident);
        return true;
    }
}
