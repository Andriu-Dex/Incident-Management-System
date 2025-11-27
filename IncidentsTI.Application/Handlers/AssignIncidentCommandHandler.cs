using IncidentsTI.Application.Commands;
using IncidentsTI.Application.Services;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IncidentsTI.Application.Handlers;

public class AssignIncidentCommandHandler : IRequestHandler<AssignIncidentCommand, bool>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IIncidentHistoryService _historyService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly INotificationService _notificationService;

    public AssignIncidentCommandHandler(
        IIncidentRepository incidentRepository,
        IIncidentHistoryService historyService,
        UserManager<ApplicationUser> userManager,
        INotificationService notificationService)
    {
        _incidentRepository = incidentRepository;
        _historyService = historyService;
        _userManager = userManager;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(AssignIncidentCommand request, CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetByIdAsync(request.IncidentId);
        
        if (incident == null)
            return false;

        // Obtener ID del técnico anterior para notificación
        var oldAssigneeId = incident.AssignedToId;
        
        // Obtener nombre del técnico anterior
        string? oldAssigneeName = null;
        if (!string.IsNullOrEmpty(incident.AssignedToId))
        {
            var oldAssignee = await _userManager.FindByIdAsync(incident.AssignedToId);
            oldAssigneeName = oldAssignee != null ? $"{oldAssignee.FirstName} {oldAssignee.LastName}" : null;
        }

        // Si AssignedToId está vacío, guardarlo como null para desasignar correctamente
        incident.AssignedToId = string.IsNullOrEmpty(request.AssignedToId) ? null : request.AssignedToId;
        incident.UpdatedAt = DateTime.UtcNow;

        await _incidentRepository.UpdateAsync(incident);
        
        // Registrar en el historial
        if (!string.IsNullOrEmpty(request.UserId) && !string.IsNullOrEmpty(request.AssignedToId))
        {
            var newAssignee = await _userManager.FindByIdAsync(request.AssignedToId);
            var newAssigneeName = newAssignee != null ? $"{newAssignee.FirstName} {newAssignee.LastName}" : "Desconocido";
            await _historyService.RecordAssignment(request.IncidentId, request.UserId, oldAssigneeName, newAssigneeName);
        }
        
        // Enviar notificaciones
        if (!string.IsNullOrEmpty(request.AssignedToId))
        {
            if (string.IsNullOrEmpty(oldAssigneeId))
            {
                // Primera asignación
                await _notificationService.NotifyIncidentAssignedAsync(incident, request.AssignedToId, request.UserId ?? "System");
            }
            else if (oldAssigneeId != request.AssignedToId)
            {
                // Reasignación
                await _notificationService.NotifyIncidentReassignedAsync(incident, oldAssigneeId, request.AssignedToId, request.UserId ?? "System");
            }
        }
        
        return true;
    }
}
