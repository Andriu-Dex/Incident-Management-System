using IncidentsTI.Application.Commands;
using IncidentsTI.Application.Services;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

public class UpdateIncidentPriorityCommandHandler : IRequestHandler<UpdateIncidentPriorityCommand, bool>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IIncidentHistoryService _historyService;

    public UpdateIncidentPriorityCommandHandler(
        IIncidentRepository incidentRepository,
        IIncidentHistoryService historyService)
    {
        _incidentRepository = incidentRepository;
        _historyService = historyService;
    }

    public async Task<bool> Handle(UpdateIncidentPriorityCommand request, CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetByIdAsync(request.IncidentId);
        
        if (incident == null)
            return false;

        var oldPriority = incident.Priority;
        incident.Priority = request.NewPriority;
        incident.UpdatedAt = DateTime.UtcNow;

        await _incidentRepository.UpdateAsync(incident);
        
        // Registrar en el historial
        if (!string.IsNullOrEmpty(request.UserId))
        {
            await _historyService.RecordPriorityChange(request.IncidentId, request.UserId, oldPriority, request.NewPriority);
        }
        
        return true;
    }
}
