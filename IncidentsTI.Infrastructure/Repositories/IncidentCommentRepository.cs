using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Interfaces;
using IncidentsTI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IncidentsTI.Infrastructure.Repositories;

public class IncidentCommentRepository : IIncidentCommentRepository
{
    private readonly ApplicationDbContext _context;

    public IncidentCommentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(IncidentComment comment)
    {
        await _context.IncidentComments.AddAsync(comment);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<IncidentComment>> GetByIncidentIdAsync(int incidentId)
    {
        return await _context.IncidentComments
            .Include(c => c.User)
            .Where(c => c.IncidentId == incidentId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<IncidentComment>> GetPublicByIncidentIdAsync(int incidentId)
    {
        return await _context.IncidentComments
            .Include(c => c.User)
            .Where(c => c.IncidentId == incidentId && !c.IsInternal)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IncidentComment?> GetByIdAsync(int id)
    {
        return await _context.IncidentComments
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}
