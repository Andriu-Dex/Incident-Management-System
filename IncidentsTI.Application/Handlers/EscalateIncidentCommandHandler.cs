using IncidentsTI.Application.Commands;
using IncidentsTI.Application.Services;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Enums;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

public class EscalateIncidentCommandHandler : IRequestHandler<EscalateIncidentCommand, bool>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IEscalationLevelRepository _escalationLevelRepository;
    private readonly IIncidentEscalationRepository _incidentEscalationRepository;
    private readonly IIncidentHistoryService _historyService;

    public EscalateIncidentCommandHandler(
        IIncidentRepository incidentRepository,
        IEscalationLevelRepository escalationLevelRepository,
        IIncidentEscalationRepository incidentEscalationRepository,
        IIncidentHistoryService historyService)
    {
        _incidentRepository = incidentRepository;
        _escalationLevelRepository = escalationLevelRepository;
        _incidentEscalationRepository = incidentEscalationRepository;
        _historyService = historyService;
    }

    public async Task<bool> Handle(EscalateIncidentCommand request, CancellationToken cancellationToken)
    {
        // Get the incident
        var incident = await _incidentRepository.GetByIdAsync(request.IncidentId);
        if (incident == null)
            return false;

        // Get the target escalation level
        var toLevel = await _escalationLevelRepository.GetByIdAsync(request.ToLevelId);
        if (toLevel == null)
            return false;

        // Get the current escalation level (if any)
        var fromLevelId = incident.CurrentEscalationLevelId;
        var fromLevelName = incident.CurrentEscalationLevel?.Name ?? "Sin nivel";

        // Create escalation record
        var escalation = new IncidentEscalation
        {
            IncidentId = request.IncidentId,
            FromUserId = request.EscalatedByUserId,
            ToUserId = request.ToUserId,
            FromLevelId = fromLevelId,
            ToLevelId = request.ToLevelId,
            Reason = request.Reason,
            Notes = request.Notes,
            EscalatedAt = DateTime.UtcNow
        };

        await _incidentEscalationRepository.AddAsync(escalation);

        // Update incident's current escalation level
        incident.CurrentEscalationLevelId = request.ToLevelId;
        
        // Change status to Escalated if not already
        var oldStatus = incident.Status;
        if (incident.Status != IncidentStatus.Escalated)
        {
            incident.Status = IncidentStatus.Escalated;
        }
        
        // If escalated to a specific user, assign them
        if (!string.IsNullOrEmpty(request.ToUserId))
        {
            incident.AssignedToId = request.ToUserId;
        }
        
        incident.UpdatedAt = DateTime.UtcNow;
        await _incidentRepository.UpdateAsync(incident);

        // Record in history
        await _historyService.RecordEscalationAsync(
            request.IncidentId,
            request.EscalatedByUserId,
            fromLevelName,
            toLevel.Name,
            request.Reason);

        return true;
    }
}
