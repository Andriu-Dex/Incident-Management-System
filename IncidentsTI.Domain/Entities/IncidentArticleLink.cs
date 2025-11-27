namespace IncidentsTI.Domain.Entities;

/// <summary>
/// Vinculación entre un incidente y un artículo de conocimiento usado para resolverlo
/// </summary>
public class IncidentArticleLink
{
    public int Id { get; set; }
    
    /// <summary>
    /// Incidente que usó el artículo
    /// </summary>
    public int IncidentId { get; set; }
    public Incident Incident { get; set; } = null!;
    
    /// <summary>
    /// Artículo usado para resolver el incidente
    /// </summary>
    public int ArticleId { get; set; }
    public KnowledgeArticle Article { get; set; } = null!;
    
    /// <summary>
    /// Usuario que vinculó el artículo al incidente
    /// </summary>
    public string LinkedByUserId { get; set; } = string.Empty;
    public ApplicationUser LinkedByUser { get; set; } = null!;
    
    /// <summary>
    /// Fecha de vinculación
    /// </summary>
    public DateTime LinkedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Si el artículo fue útil para resolver el incidente
    /// </summary>
    public bool WasHelpful { get; set; } = true;
    
    /// <summary>
    /// Comentario opcional sobre cómo se usó el artículo
    /// </summary>
    public string? Notes { get; set; }
}
