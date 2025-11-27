using IncidentsTI.Domain.Enums;

namespace IncidentsTI.Domain.Entities;

/// <summary>
/// Artículo de la base de conocimiento con soluciones documentadas
/// </summary>
public class KnowledgeArticle
{
    public int Id { get; set; }
    
    /// <summary>
    /// Título del artículo/solución
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Servicio relacionado (del catálogo)
    /// </summary>
    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;
    
    /// <summary>
    /// Tipo de incidente que resuelve
    /// </summary>
    public IncidentType IncidentType { get; set; }
    
    /// <summary>
    /// Descripción del problema
    /// </summary>
    public string ProblemDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// Recomendaciones adicionales
    /// </summary>
    public string? Recommendations { get; set; }
    
    /// <summary>
    /// Tiempo estimado de resolución en minutos
    /// </summary>
    public int? EstimatedResolutionTimeMinutes { get; set; }
    
    /// <summary>
    /// Autor del artículo (técnico que lo creó)
    /// </summary>
    public string AuthorId { get; set; } = string.Empty;
    public ApplicationUser Author { get; set; } = null!;
    
    /// <summary>
    /// Incidente que originó esta solución (opcional)
    /// </summary>
    public int? OriginIncidentId { get; set; }
    public Incident? OriginIncident { get; set; }
    
    /// <summary>
    /// Número de veces que se ha usado este artículo
    /// </summary>
    public int UsageCount { get; set; } = 0;
    
    /// <summary>
    /// Si el artículo está activo/visible
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navegación
    public ICollection<SolutionStep> Steps { get; set; } = new List<SolutionStep>();
    public ICollection<ArticleKeyword> Keywords { get; set; } = new List<ArticleKeyword>();
    public ICollection<IncidentArticleLink> LinkedIncidents { get; set; } = new List<IncidentArticleLink>();
}
