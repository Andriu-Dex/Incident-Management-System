using IncidentsTI.Application.Commands;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

public class UpdateIncidentTypeCommandHandler : IRequestHandler<UpdateIncidentTypeCommand, bool>
{
    private readonly IIncidentRepository _incidentRepository;

    public UpdateIncidentTypeCommandHandler(IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<bool> Handle(UpdateIncidentTypeCommand request, CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetByIdAsync(request.IncidentId);
        
        if (incident == null)
            return false;

        incident.Type = request.NewType;
        incident.UpdatedAt = DateTime.UtcNow;

        await _incidentRepository.UpdateAsync(incident);
        return true;
    }
}
