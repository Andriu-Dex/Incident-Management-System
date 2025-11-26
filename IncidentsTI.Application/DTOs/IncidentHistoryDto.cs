using IncidentsTI.Domain.Enums;

namespace IncidentsTI.Application.DTOs;

/// <summary>
/// DTO para mostrar el historial de un incidente
/// </summary>
public class IncidentHistoryDto
{
    public int Id { get; set; }
    public int IncidentId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public HistoryAction Action { get; set; }
    public string ActionDescription { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Description { get; set; }
    public DateTime Timestamp { get; set; }
    
    /// <summary>
    /// Texto formateado para mostrar en la UI
    /// </summary>
    public string FormattedAction => GetFormattedAction();
    
    private string GetFormattedAction()
    {
        return Action switch
        {
            HistoryAction.Created => "creó el incidente",
            HistoryAction.StatusChanged => $"cambió el estado de '{OldValue}' a '{NewValue}'",
            HistoryAction.PriorityChanged => $"cambió la prioridad de '{OldValue}' a '{NewValue}'",
            HistoryAction.TypeChanged => $"cambió el tipo de '{OldValue}' a '{NewValue}'",
            HistoryAction.ServiceChanged => $"cambió el servicio de '{OldValue}' a '{NewValue}'",
            HistoryAction.Assigned => $"asignó el incidente a '{NewValue}'",
            HistoryAction.Reassigned => $"reasignó de '{OldValue}' a '{NewValue}'",
            HistoryAction.CommentAdded => "agregó un comentario",
            HistoryAction.Escalated => "escaló el incidente",
            HistoryAction.DetailsUpdated => "actualizó los detalles del incidente",
            _ => "realizó una acción"
        };
    }
}
