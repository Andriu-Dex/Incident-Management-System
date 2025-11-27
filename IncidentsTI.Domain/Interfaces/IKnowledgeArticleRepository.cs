using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Enums;

namespace IncidentsTI.Domain.Interfaces;

/// <summary>
/// Repositorio para artículos de la base de conocimiento
/// </summary>
public interface IKnowledgeArticleRepository
{
    /// <summary>
    /// Obtener artículo por ID con todas sus relaciones
    /// </summary>
    Task<KnowledgeArticle?> GetByIdAsync(int id);
    
    /// <summary>
    /// Obtener artículo por ID incluyendo pasos y keywords
    /// </summary>
    Task<KnowledgeArticle?> GetByIdWithDetailsAsync(int id);
    
    /// <summary>
    /// Obtener todos los artículos activos
    /// </summary>
    Task<IEnumerable<KnowledgeArticle>> GetAllActiveAsync();
    
    /// <summary>
    /// Obtener todos los artículos (incluidos inactivos) para administración
    /// </summary>
    Task<IEnumerable<KnowledgeArticle>> GetAllAsync();
    
    /// <summary>
    /// Buscar artículos por palabra clave
    /// </summary>
    Task<IEnumerable<KnowledgeArticle>> SearchByKeywordAsync(string keyword);
    
    /// <summary>
    /// Buscar artículos por servicio
    /// </summary>
    Task<IEnumerable<KnowledgeArticle>> GetByServiceIdAsync(int serviceId);
    
    /// <summary>
    /// Buscar artículos por tipo de incidente
    /// </summary>
    Task<IEnumerable<KnowledgeArticle>> GetByIncidentTypeAsync(IncidentType type);
    
    /// <summary>
    /// Buscar artículos con múltiples filtros
    /// </summary>
    Task<IEnumerable<KnowledgeArticle>> SearchAsync(string? keyword, int? serviceId, IncidentType? incidentType);
    
    /// <summary>
    /// Obtener artículos relacionados (mismo servicio o tipo de incidente)
    /// </summary>
    Task<IEnumerable<KnowledgeArticle>> GetRelatedArticlesAsync(int serviceId, IncidentType incidentType, int excludeArticleId = 0);
    
    /// <summary>
    /// Obtener artículos más usados
    /// </summary>
    Task<IEnumerable<KnowledgeArticle>> GetMostUsedAsync(int count = 5);
    
    /// <summary>
    /// Obtener artículos creados por un autor específico
    /// </summary>
    Task<IEnumerable<KnowledgeArticle>> GetByAuthorAsync(string authorId);
    
    /// <summary>
    /// Agregar nuevo artículo
    /// </summary>
    Task<KnowledgeArticle> AddAsync(KnowledgeArticle article);
    
    /// <summary>
    /// Actualizar artículo existente
    /// </summary>
    Task UpdateAsync(KnowledgeArticle article);
    
    /// <summary>
    /// Incrementar contador de uso
    /// </summary>
    Task IncrementUsageCountAsync(int articleId);
    
    /// <summary>
    /// Obtener vínculos de un artículo con incidentes
    /// </summary>
    Task<IEnumerable<IncidentArticleLink>> GetArticleLinksAsync(int articleId);
    
    /// <summary>
    /// Obtener vínculos de un incidente con artículos
    /// </summary>
    Task<IEnumerable<IncidentArticleLink>> GetIncidentLinksAsync(int incidentId);
    
    /// <summary>
    /// Agregar vínculo entre incidente y artículo
    /// </summary>
    Task<IncidentArticleLink> AddLinkAsync(IncidentArticleLink link);
    
    /// <summary>
    /// Verificar si existe vínculo entre incidente y artículo
    /// </summary>
    Task<bool> LinkExistsAsync(int incidentId, int articleId);
}
