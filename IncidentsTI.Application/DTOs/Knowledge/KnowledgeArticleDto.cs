using IncidentsTI.Domain.Enums;

namespace IncidentsTI.Application.DTOs.Knowledge;

/// <summary>
/// DTO completo para un artículo de conocimiento
/// </summary>
public class KnowledgeArticleDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    
    // Servicio
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string ServiceCategory { get; set; } = string.Empty;
    
    // Tipo de incidente
    public IncidentType IncidentType { get; set; }
    public string IncidentTypeName { get; set; } = string.Empty;
    
    // Contenido
    public string ProblemDescription { get; set; } = string.Empty;
    public string? Recommendations { get; set; }
    public int? EstimatedResolutionTimeMinutes { get; set; }
    
    /// <summary>
    /// Tiempo formateado (ej: "15 min", "1h 30min")
    /// </summary>
    public string EstimatedTimeFormatted => FormatTime(EstimatedResolutionTimeMinutes);
    
    // Autor
    public string AuthorId { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    
    // Incidente origen
    public int? OriginIncidentId { get; set; }
    public string? OriginIncidentTicket { get; set; }
    
    // Estadísticas
    public int UsageCount { get; set; }
    public bool IsActive { get; set; }
    
    // Fechas
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Relaciones
    public List<SolutionStepDto> Steps { get; set; } = new();
    public List<string> Keywords { get; set; } = new();
    
    private static string FormatTime(int? minutes)
    {
        if (!minutes.HasValue || minutes.Value <= 0)
            return "No estimado";
            
        if (minutes.Value < 60)
            return $"{minutes.Value} min";
            
        var hours = minutes.Value / 60;
        var mins = minutes.Value % 60;
        
        if (mins == 0)
            return $"{hours}h";
            
        return $"{hours}h {mins}min";
    }
}

/// <summary>
/// DTO para listado de artículos (más ligero)
/// </summary>
public class KnowledgeArticleListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string ServiceCategory { get; set; } = string.Empty;
    public IncidentType IncidentType { get; set; }
    public string IncidentTypeName { get; set; } = string.Empty;
    public string ProblemDescription { get; set; } = string.Empty;
    public int? EstimatedResolutionTimeMinutes { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public int UsageCount { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Keywords { get; set; } = new();
    
    public string EstimatedTimeFormatted => FormatTime(EstimatedResolutionTimeMinutes);
    
    private static string FormatTime(int? minutes)
    {
        if (!minutes.HasValue || minutes.Value <= 0)
            return "—";
            
        if (minutes.Value < 60)
            return $"{minutes.Value} min";
            
        var hours = minutes.Value / 60;
        var mins = minutes.Value % 60;
        
        if (mins == 0)
            return $"{hours}h";
            
        return $"{hours}h {mins}min";
    }
}
