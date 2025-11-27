using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Enums;
using IncidentsTI.Domain.Interfaces;

namespace IncidentsTI.Application.Services;

/// <summary>
/// Servicio para registrar acciones en el historial de incidentes
/// </summary>
public interface IIncidentHistoryService
{
    Task RecordCreation(int incidentId, string userId);
    Task RecordStatusChange(int incidentId, string userId, IncidentStatus oldStatus, IncidentStatus newStatus);
    Task RecordPriorityChange(int incidentId, string userId, IncidentPriority oldPriority, IncidentPriority newPriority);
    Task RecordTypeChange(int incidentId, string userId, IncidentType oldType, IncidentType newType);
    Task RecordServiceChange(int incidentId, string userId, string oldServiceName, string newServiceName);
    Task RecordAssignment(int incidentId, string userId, string? oldAssigneeName, string newAssigneeName);
    Task RecordEscalationAsync(int incidentId, string userId, string fromLevelName, string toLevelName, string reason);
}

public class IncidentHistoryService : IIncidentHistoryService
{
    private readonly IIncidentHistoryRepository _historyRepository;

    public IncidentHistoryService(IIncidentHistoryRepository historyRepository)
    {
        _historyRepository = historyRepository;
    }

    public async Task RecordCreation(int incidentId, string userId)
    {
        var history = new IncidentHistory
        {
            IncidentId = incidentId,
            UserId = userId,
            Action = HistoryAction.Created,
            NewValue = "Incidente creado",
            Timestamp = DateTime.UtcNow
        };

        await _historyRepository.AddAsync(history);
    }

    public async Task RecordStatusChange(int incidentId, string userId, IncidentStatus oldStatus, IncidentStatus newStatus)
    {
        var history = new IncidentHistory
        {
            IncidentId = incidentId,
            UserId = userId,
            Action = HistoryAction.StatusChanged,
            OldValue = GetStatusDisplayName(oldStatus),
            NewValue = GetStatusDisplayName(newStatus),
            Timestamp = DateTime.UtcNow
        };

        await _historyRepository.AddAsync(history);
    }

    public async Task RecordPriorityChange(int incidentId, string userId, IncidentPriority oldPriority, IncidentPriority newPriority)
    {
        var history = new IncidentHistory
        {
            IncidentId = incidentId,
            UserId = userId,
            Action = HistoryAction.PriorityChanged,
            OldValue = GetPriorityDisplayName(oldPriority),
            NewValue = GetPriorityDisplayName(newPriority),
            Timestamp = DateTime.UtcNow
        };

        await _historyRepository.AddAsync(history);
    }

    public async Task RecordTypeChange(int incidentId, string userId, IncidentType oldType, IncidentType newType)
    {
        var history = new IncidentHistory
        {
            IncidentId = incidentId,
            UserId = userId,
            Action = HistoryAction.TypeChanged,
            OldValue = GetTypeDisplayName(oldType),
            NewValue = GetTypeDisplayName(newType),
            Timestamp = DateTime.UtcNow
        };

        await _historyRepository.AddAsync(history);
    }

    public async Task RecordServiceChange(int incidentId, string userId, string oldServiceName, string newServiceName)
    {
        var history = new IncidentHistory
        {
            IncidentId = incidentId,
            UserId = userId,
            Action = HistoryAction.ServiceChanged,
            OldValue = oldServiceName,
            NewValue = newServiceName,
            Timestamp = DateTime.UtcNow
        };

        await _historyRepository.AddAsync(history);
    }

    public async Task RecordAssignment(int incidentId, string userId, string? oldAssigneeName, string newAssigneeName)
    {
        var history = new IncidentHistory
        {
            IncidentId = incidentId,
            UserId = userId,
            Action = string.IsNullOrEmpty(oldAssigneeName) ? HistoryAction.Assigned : HistoryAction.Reassigned,
            OldValue = oldAssigneeName,
            NewValue = newAssigneeName,
            Timestamp = DateTime.UtcNow
        };

        await _historyRepository.AddAsync(history);
    }

    public async Task RecordEscalationAsync(int incidentId, string userId, string fromLevelName, string toLevelName, string reason)
    {
        var history = new IncidentHistory
        {
            IncidentId = incidentId,
            UserId = userId,
            Action = HistoryAction.Escalated,
            OldValue = fromLevelName,
            NewValue = toLevelName,
            Description = reason,
            Timestamp = DateTime.UtcNow
        };

        await _historyRepository.AddAsync(history);
    }

    private string GetStatusDisplayName(IncidentStatus status) => status switch
    {
        IncidentStatus.Open => "Abierto",
        IncidentStatus.InProgress => "En Progreso",
        IncidentStatus.Escalated => "Escalado",
        IncidentStatus.Resolved => "Resuelto",
        IncidentStatus.Closed => "Cerrado",
        _ => status.ToString()
    };

    private string GetPriorityDisplayName(IncidentPriority priority) => priority switch
    {
        IncidentPriority.Low => "Baja",
        IncidentPriority.Medium => "Media",
        IncidentPriority.High => "Alta",
        IncidentPriority.Critical => "Crítica",
        _ => priority.ToString()
    };

    private string GetTypeDisplayName(IncidentType type) => type switch
    {
        IncidentType.Failure => "Falla",
        IncidentType.Query => "Consulta",
        IncidentType.Request => "Requerimiento",
        _ => type.ToString()
    };
}
