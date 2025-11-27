using IncidentsTI.Application.Commands;
using IncidentsTI.Application.Services;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

public class UpdateIncidentStatusCommandHandler : IRequestHandler<UpdateIncidentStatusCommand, bool>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IIncidentHistoryService _historyService;
    private readonly INotificationService _notificationService;

    public UpdateIncidentStatusCommandHandler(
        IIncidentRepository incidentRepository,
        IIncidentHistoryService historyService,
        INotificationService notificationService)
    {
        _incidentRepository = incidentRepository;
        _historyService = historyService;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(UpdateIncidentStatusCommand request, CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetByIdAsync(request.IncidentId);
        
        if (incident == null)
            return false;

        var oldStatus = incident.Status;
        incident.Status = request.NewStatus;
        incident.UpdatedAt = DateTime.UtcNow;

        await _incidentRepository.UpdateAsync(incident);
        
        // Registrar en el historial
        if (!string.IsNullOrEmpty(request.UserId))
        {
            await _historyService.RecordStatusChange(request.IncidentId, request.UserId, oldStatus, request.NewStatus);
        }
        
        // Enviar notificación de cambio de estado
        await _notificationService.NotifyStatusChangedAsync(incident, oldStatus, request.NewStatus, request.UserId ?? "System");
        
        return true;
    }
}
