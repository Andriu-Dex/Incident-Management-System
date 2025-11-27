using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Enums;
using IncidentsTI.Domain.Interfaces;
using IncidentsTI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IncidentsTI.Infrastructure.Repositories;

/// <summary>
/// Repositorio para artículos de la base de conocimiento
/// </summary>
public class KnowledgeArticleRepository : IKnowledgeArticleRepository
{
    private readonly ApplicationDbContext _context;

    public KnowledgeArticleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<KnowledgeArticle?> GetByIdAsync(int id)
    {
        return await _context.KnowledgeArticles
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<KnowledgeArticle?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.KnowledgeArticles
            .Include(a => a.Steps)
            .Include(a => a.Keywords)
            .Include(a => a.LinkedIncidents)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<KnowledgeArticle>> GetAllActiveAsync()
    {
        return await _context.KnowledgeArticles
            .Include(a => a.Keywords)
            .Where(a => a.IsActive)
            .OrderByDescending(a => a.UsageCount)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<KnowledgeArticle>> GetAllAsync()
    {
        return await _context.KnowledgeArticles
            .Include(a => a.Keywords)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<KnowledgeArticle>> SearchByKeywordAsync(string keyword)
    {
        var lowerKeyword = keyword.ToLower();
        
        return await _context.KnowledgeArticles
            .Include(a => a.Keywords)
            .Where(a => a.IsActive && (
                a.Title.ToLower().Contains(lowerKeyword) ||
                a.ProblemDescription.ToLower().Contains(lowerKeyword) ||
                a.Keywords.Any(k => k.Keyword.Contains(lowerKeyword))
            ))
            .OrderByDescending(a => a.UsageCount)
            .ToListAsync();
    }

    public async Task<IEnumerable<KnowledgeArticle>> GetByServiceIdAsync(int serviceId)
    {
        return await _context.KnowledgeArticles
            .Include(a => a.Keywords)
            .Where(a => a.IsActive && a.ServiceId == serviceId)
            .OrderByDescending(a => a.UsageCount)
            .ToListAsync();
    }

    public async Task<IEnumerable<KnowledgeArticle>> GetByIncidentTypeAsync(IncidentType type)
    {
        return await _context.KnowledgeArticles
            .Include(a => a.Keywords)
            .Where(a => a.IsActive && a.IncidentType == type)
            .OrderByDescending(a => a.UsageCount)
            .ToListAsync();
    }

    public async Task<IEnumerable<KnowledgeArticle>> SearchAsync(string? keyword, int? serviceId, IncidentType? incidentType)
    {
        var query = _context.KnowledgeArticles
            .Include(a => a.Keywords)
            .AsQueryable();

        // Filtrar por palabra clave
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var lowerKeyword = keyword.ToLower();
            query = query.Where(a =>
                a.Title.ToLower().Contains(lowerKeyword) ||
                a.ProblemDescription.ToLower().Contains(lowerKeyword) ||
                a.Keywords.Any(k => k.Keyword.Contains(lowerKeyword))
            );
        }

        // Filtrar por servicio
        if (serviceId.HasValue)
        {
            query = query.Where(a => a.ServiceId == serviceId.Value);
        }

        // Filtrar por tipo de incidente
        if (incidentType.HasValue)
        {
            query = query.Where(a => a.IncidentType == incidentType.Value);
        }

        return await query
            .OrderByDescending(a => a.UsageCount)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<KnowledgeArticle>> GetRelatedArticlesAsync(int serviceId, IncidentType incidentType, int excludeArticleId = 0)
    {
        return await _context.KnowledgeArticles
            .Include(a => a.Keywords)
            .Where(a => a.IsActive && 
                        a.Id != excludeArticleId &&
                        (a.ServiceId == serviceId || a.IncidentType == incidentType))
            .OrderByDescending(a => a.ServiceId == serviceId && a.IncidentType == incidentType) // Priorizar coincidencia exacta
            .ThenByDescending(a => a.UsageCount)
            .Take(10)
            .ToListAsync();
    }

    public async Task<IEnumerable<KnowledgeArticle>> GetMostUsedAsync(int count = 5)
    {
        return await _context.KnowledgeArticles
            .Include(a => a.Keywords)
            .Where(a => a.IsActive)
            .OrderByDescending(a => a.UsageCount)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<KnowledgeArticle>> GetByAuthorAsync(string authorId)
    {
        return await _context.KnowledgeArticles
            .Include(a => a.Keywords)
            .Where(a => a.AuthorId == authorId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<KnowledgeArticle> AddAsync(KnowledgeArticle article)
    {
        _context.KnowledgeArticles.Add(article);
        await _context.SaveChangesAsync();
        return article;
    }

    public async Task UpdateAsync(KnowledgeArticle article)
    {
        _context.KnowledgeArticles.Update(article);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(KnowledgeArticle article)
    {
        // Eliminar primero los keywords y steps relacionados
        var keywords = _context.ArticleKeywords.Where(k => k.ArticleId == article.Id);
        _context.ArticleKeywords.RemoveRange(keywords);
        
        var steps = _context.SolutionSteps.Where(s => s.ArticleId == article.Id);
        _context.SolutionSteps.RemoveRange(steps);
        
        // Eliminar vínculos con incidentes
        var links = _context.IncidentArticleLinks.Where(l => l.ArticleId == article.Id);
        _context.IncidentArticleLinks.RemoveRange(links);
        
        // Eliminar el artículo
        _context.KnowledgeArticles.Remove(article);
        await _context.SaveChangesAsync();
    }

    public async Task IncrementUsageCountAsync(int articleId)
    {
        var article = await _context.KnowledgeArticles.FindAsync(articleId);
        if (article != null)
        {
            article.UsageCount++;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<IncidentArticleLink>> GetArticleLinksAsync(int articleId)
    {
        return await _context.IncidentArticleLinks
            .Where(l => l.ArticleId == articleId)
            .OrderByDescending(l => l.LinkedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<IncidentArticleLink>> GetIncidentLinksAsync(int incidentId)
    {
        return await _context.IncidentArticleLinks
            .Where(l => l.IncidentId == incidentId)
            .OrderByDescending(l => l.LinkedAt)
            .ToListAsync();
    }

    public async Task<IncidentArticleLink> AddLinkAsync(IncidentArticleLink link)
    {
        _context.IncidentArticleLinks.Add(link);
        await _context.SaveChangesAsync();
        return link;
    }

    public async Task<bool> LinkExistsAsync(int incidentId, int articleId)
    {
        return await _context.IncidentArticleLinks
            .AnyAsync(l => l.IncidentId == incidentId && l.ArticleId == articleId);
    }
}
