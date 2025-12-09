using IncidentsTI.Application.Commands;
using IncidentsTI.Application.Services;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Enums;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

/// <summary>
/// Handler para escalar incidentes a un nivel superior.
/// Al escalar, el incidente se LIBERA y queda disponible para que
/// personal del nivel superior lo reclame.
/// </summary>
public class EscalateIncidentCommandHandler : IRequestHandler<EscalateIncidentCommand, bool>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IEscalationLevelRepository _escalationLevelRepository;
    private readonly IIncidentEscalationRepository _incidentEscalationRepository;
    private readonly IIncidentHistoryService _historyService;
    private readonly INotificationService _notificationService;

    public EscalateIncidentCommandHandler(
        IIncidentRepository incidentRepository,
        IEscalationLevelRepository escalationLevelRepository,
        IIncidentEscalationRepository incidentEscalationRepository,
        IIncidentHistoryService historyService,
        INotificationService notificationService)
    {
        _incidentRepository = incidentRepository;
        _escalationLevelRepository = escalationLevelRepository;
        _incidentEscalationRepository = incidentEscalationRepository;
        _historyService = historyService;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(EscalateIncidentCommand request, CancellationToken cancellationToken)
    {
        // Obtener el incidente
        var incident = await _incidentRepository.GetByIdAsync(request.IncidentId);
        if (incident == null)
            return false;

        // Obtener el nivel destino
        var toLevel = await _escalationLevelRepository.GetByIdAsync(request.ToLevelId);
        if (toLevel == null)
            return false;

        // Validar que el nivel destino sea mayor al actual
        var currentLevelOrder = incident.CurrentEscalationLevelId ?? 0;
        if (toLevel.Order <= currentLevelOrder)
            return false; // No se puede escalar a un nivel igual o inferior

        // Obtener información del nivel actual
        var fromLevelId = incident.CurrentEscalationLevelId;
        var fromLevelName = incident.CurrentEscalationLevel?.Name ?? "Sin nivel";

        // Crear registro de escalamiento
        var escalation = new IncidentEscalation
        {
            IncidentId = request.IncidentId,
            FromUserId = request.EscalatedByUserId,
            ToUserId = null, // Se libera, no se asigna a nadie específico
            FromLevelId = fromLevelId,
            ToLevelId = request.ToLevelId,
            Reason = request.Reason,
            Notes = request.Notes,
            EscalatedAt = DateTime.UtcNow
        };

        await _incidentEscalationRepository.AddAsync(escalation);
        
        // Cargar el nivel para la notificación
        escalation.ToLevel = toLevel;

        // ═══════════════════════════════════════════════════════════════════
        // CAMBIOS CLAVE: Liberar incidente y registrar quién escaló
        // ═══════════════════════════════════════════════════════════════════
        
        // Guardar quién escaló (para que pueda ver en solo lectura)
        incident.EscalatedByUserId = request.EscalatedByUserId;
        
        // LIBERAR el incidente - ya no está asignado a nadie
        // Esto permite que personal del nivel superior lo reclame
        incident.AssignedToId = null;
        
        // Actualizar nivel y estado
        incident.CurrentEscalationLevelId = request.ToLevelId;
        incident.Status = IncidentStatus.Escalated;
        incident.UpdatedAt = DateTime.UtcNow;
        
        await _incidentRepository.UpdateAsync(incident);

        // Registrar en historial
        await _historyService.RecordEscalationAsync(
            request.IncidentId,
            request.EscalatedByUserId,
            fromLevelName,
            toLevel.Name,
            request.Reason);
            
        // Enviar notificación de escalamiento
        await _notificationService.NotifyIncidentEscalatedAsync(incident, escalation);

        return true;
    }
}
