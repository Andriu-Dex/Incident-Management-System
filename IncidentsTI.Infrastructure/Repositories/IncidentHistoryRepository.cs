using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Interfaces;
using IncidentsTI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IncidentsTI.Infrastructure.Repositories;

public class IncidentHistoryRepository : IIncidentHistoryRepository
{
    private readonly ApplicationDbContext _context;

    public IncidentHistoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(IncidentHistory history)
    {
        await _context.IncidentHistories.AddAsync(history);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<IncidentHistory>> GetByIncidentIdAsync(int incidentId)
    {
        return await _context.IncidentHistories
            .Include(h => h.User)
            .Where(h => h.IncidentId == incidentId)
            .OrderByDescending(h => h.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<IncidentHistory>> GetRecentByIncidentIdAsync(int incidentId, int count = 10)
    {
        return await _context.IncidentHistories
            .Include(h => h.User)
            .Where(h => h.IncidentId == incidentId)
            .OrderByDescending(h => h.Timestamp)
            .Take(count)
            .ToListAsync();
    }
}
