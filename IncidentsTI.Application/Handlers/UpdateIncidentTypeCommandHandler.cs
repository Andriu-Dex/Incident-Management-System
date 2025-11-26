using IncidentsTI.Application.Commands;
using IncidentsTI.Application.Services;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

public class UpdateIncidentTypeCommandHandler : IRequestHandler<UpdateIncidentTypeCommand, bool>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IIncidentHistoryService _historyService;

    public UpdateIncidentTypeCommandHandler(
        IIncidentRepository incidentRepository,
        IIncidentHistoryService historyService)
    {
        _incidentRepository = incidentRepository;
        _historyService = historyService;
    }

    public async Task<bool> Handle(UpdateIncidentTypeCommand request, CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetByIdAsync(request.IncidentId);
        
        if (incident == null)
            return false;

        var oldType = incident.Type;
        incident.Type = request.NewType;
        incident.UpdatedAt = DateTime.UtcNow;

        await _incidentRepository.UpdateAsync(incident);
        
        // Registrar en el historial
        if (!string.IsNullOrEmpty(request.UserId))
        {
            await _historyService.RecordTypeChange(request.IncidentId, request.UserId, oldType, request.NewType);
        }
        
        return true;
    }
}
