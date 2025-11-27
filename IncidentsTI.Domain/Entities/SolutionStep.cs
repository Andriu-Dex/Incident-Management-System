namespace IncidentsTI.Domain.Entities;

/// <summary>
/// Paso individual de una solución en un artículo de conocimiento
/// </summary>
public class SolutionStep
{
    public int Id { get; set; }
    
    /// <summary>
    /// Artículo al que pertenece
    /// </summary>
    public int ArticleId { get; set; }
    public KnowledgeArticle Article { get; set; } = null!;
    
    /// <summary>
    /// Número de orden del paso (1, 2, 3...)
    /// </summary>
    public int StepNumber { get; set; }
    
    /// <summary>
    /// Título o resumen breve del paso
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Descripción detallada del paso
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Nota adicional o advertencia (opcional)
    /// </summary>
    public string? Note { get; set; }
}
