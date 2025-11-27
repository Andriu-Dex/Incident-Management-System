namespace IncidentsTI.Application.DTOs.Escalation;

public class IncidentEscalationDto
{
    public int Id { get; set; }
    public int IncidentId { get; set; }
    
    // Usuario que escaló
    public string FromUserId { get; set; } = string.Empty;
    public string FromUserName { get; set; } = string.Empty;
    
    // Usuario al que se escaló (opcional)
    public string? ToUserId { get; set; }
    public string? ToUserName { get; set; }
    
    // Niveles
    public int? FromLevelId { get; set; }
    public string? FromLevelName { get; set; }
    public int ToLevelId { get; set; }
    public string ToLevelName { get; set; } = string.Empty;
    
    // Detalles
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime EscalatedAt { get; set; }
    
    // Descripción formateada para mostrar
    public string FormattedDescription { get; set; } = string.Empty;
}
