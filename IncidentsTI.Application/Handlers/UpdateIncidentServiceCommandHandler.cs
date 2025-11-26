using IncidentsTI.Application.Commands;
using IncidentsTI.Application.Services;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

public class UpdateIncidentServiceCommandHandler : IRequestHandler<UpdateIncidentServiceCommand, bool>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IIncidentHistoryService _historyService;

    public UpdateIncidentServiceCommandHandler(
        IIncidentRepository incidentRepository,
        IServiceRepository serviceRepository,
        IIncidentHistoryService historyService)
    {
        _incidentRepository = incidentRepository;
        _serviceRepository = serviceRepository;
        _historyService = historyService;
    }

    public async Task<bool> Handle(UpdateIncidentServiceCommand request, CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetByIdAsync(request.IncidentId);
        
        if (incident == null)
            return false;

        // Obtener nombre del servicio anterior
        var oldService = await _serviceRepository.GetByIdAsync(incident.ServiceId);
        var oldServiceName = oldService?.Name ?? "Desconocido";

        // Verificar que el nuevo servicio existe
        var newService = await _serviceRepository.GetByIdAsync(request.NewServiceId);
        if (newService == null)
            return false;

        incident.ServiceId = request.NewServiceId;
        incident.UpdatedAt = DateTime.UtcNow;

        await _incidentRepository.UpdateAsync(incident);
        
        // Registrar en el historial
        if (!string.IsNullOrEmpty(request.UserId))
        {
            await _historyService.RecordServiceChange(request.IncidentId, request.UserId, oldServiceName, newService.Name);
        }
        
        return true;
    }
}
