using IncidentsTI.Application.Commands;
using IncidentsTI.Application.Common;
using IncidentsTI.Application.Services;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Enums;
using IncidentsTI.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IncidentsTI.Application.Handlers;

/// <summary>
/// Handler para el comando de reclamo de incidentes
/// Implementa el sistema "first-come-first-serve" con niveles de soporte
/// </summary>
public class ClaimIncidentCommandHandler : IRequestHandler<ClaimIncidentCommand, ClaimIncidentResult>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IIncidentHistoryService _historyService;
    private readonly INotificationService _notificationService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ClaimIncidentCommandHandler(
        IIncidentRepository incidentRepository,
        IIncidentHistoryService historyService,
        INotificationService notificationService,
        UserManager<ApplicationUser> userManager)
    {
        _incidentRepository = incidentRepository;
        _historyService = historyService;
        _notificationService = notificationService;
        _userManager = userManager;
    }

    public async Task<ClaimIncidentResult> Handle(ClaimIncidentCommand request, CancellationToken cancellationToken)
    {
        // Obtener el incidente
        var incident = await _incidentRepository.GetByIdAsync(request.IncidentId);
        
        if (incident == null)
            return ClaimIncidentResult.Fail("Incidente no encontrado");

        // Verificar que no esté asignado a nadie
        if (!string.IsNullOrEmpty(incident.AssignedToId))
            return ClaimIncidentResult.Fail("Este incidente ya ha sido reclamado por otro usuario");

        // Verificar que el incidente esté en estado Open o Escalated (sin asignar)
        if (incident.Status != IncidentStatus.Open && incident.Status != IncidentStatus.Escalated)
            return ClaimIncidentResult.Fail("Solo se pueden reclamar incidentes en estado 'Abierto' o 'Escalado'");

        // Obtener el usuario que reclama y sus roles
        var claimer = await _userManager.FindByIdAsync(request.ClaimedByUserId);
        if (claimer == null)
            return ClaimIncidentResult.Fail("Usuario no encontrado");

        var roles = await _userManager.GetRolesAsync(claimer);
        var userLevel = UserLevelResolver.GetUserLevel(roles);

        // Verificar que el usuario sea personal de soporte
        if (userLevel == 0)
            return ClaimIncidentResult.Fail("No tienes permisos para reclamar incidentes");

        // Verificar que el usuario tenga nivel suficiente para este incidente
        if (!UserLevelResolver.CanAccessLevel(userLevel, incident.CurrentEscalationLevelId))
            return ClaimIncidentResult.Fail($"Este incidente requiere nivel {incident.CurrentEscalationLevelId}. Tu nivel es {userLevel}.");

        var claimerName = $"{claimer.FirstName} {claimer.LastName}";

        // Asignar al usuario que reclama
        incident.AssignedToId = request.ClaimedByUserId;
        incident.Status = IncidentStatus.InProgress;
        
        // Si es un incidente nuevo (sin nivel), asignar el nivel según el rol del usuario
        if (!incident.CurrentEscalationLevelId.HasValue)
        {
            incident.CurrentEscalationLevelId = userLevel;
        }
        
        incident.UpdatedAt = DateTime.UtcNow;

        await _incidentRepository.UpdateAsync(incident);
        
        // Registrar en el historial
        await _historyService.RecordClaim(request.IncidentId, request.ClaimedByUserId, claimerName);
        
        // Notificar al creador del incidente
        await _notificationService.NotifyIncidentClaimedAsync(incident, request.ClaimedByUserId);
        
        return ClaimIncidentResult.Ok();
    }
}
