using IncidentsTI.Application.Commands;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

public class UpdateIncidentServiceCommandHandler : IRequestHandler<UpdateIncidentServiceCommand, bool>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IServiceRepository _serviceRepository;

    public UpdateIncidentServiceCommandHandler(
        IIncidentRepository incidentRepository,
        IServiceRepository serviceRepository)
    {
        _incidentRepository = incidentRepository;
        _serviceRepository = serviceRepository;
    }

    public async Task<bool> Handle(UpdateIncidentServiceCommand request, CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetByIdAsync(request.IncidentId);
        
        if (incident == null)
            return false;

        // Verificar que el nuevo servicio existe
        var service = await _serviceRepository.GetByIdAsync(request.NewServiceId);
        if (service == null)
            return false;

        incident.ServiceId = request.NewServiceId;
        incident.UpdatedAt = DateTime.UtcNow;

        await _incidentRepository.UpdateAsync(incident);
        return true;
    }
}
