namespace IncidentsTI.Domain.Entities;

/// <summary>
/// Palabra clave para búsqueda en artículos de conocimiento
/// </summary>
public class ArticleKeyword
{
    public int Id { get; set; }
    
    /// <summary>
    /// Artículo al que pertenece
    /// </summary>
    public int ArticleId { get; set; }
    public KnowledgeArticle Article { get; set; } = null!;
    
    /// <summary>
    /// Palabra clave (en minúsculas para búsqueda)
    /// </summary>
    public string Keyword { get; set; } = string.Empty;
}
