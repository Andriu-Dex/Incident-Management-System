using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Interfaces;
using IncidentsTI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IncidentsTI.Infrastructure.Repositories;

public class EscalationLevelRepository : IEscalationLevelRepository
{
    private readonly ApplicationDbContext _context;

    public EscalationLevelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EscalationLevel?> GetByIdAsync(int id)
    {
        return await _context.EscalationLevels
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<EscalationLevel>> GetAllAsync()
    {
        return await _context.EscalationLevels
            .OrderBy(e => e.Order)
            .ToListAsync();
    }

    public async Task<List<EscalationLevel>> GetActiveAsync()
    {
        return await _context.EscalationLevels
            .Where(e => e.IsActive)
            .OrderBy(e => e.Order)
            .ToListAsync();
    }

    public async Task<EscalationLevel?> GetByOrderAsync(int order)
    {
        return await _context.EscalationLevels
            .FirstOrDefaultAsync(e => e.Order == order && e.IsActive);
    }

    public async Task<EscalationLevel?> GetNextLevelAsync(int currentOrder)
    {
        return await _context.EscalationLevels
            .Where(e => e.Order > currentOrder && e.IsActive)
            .OrderBy(e => e.Order)
            .FirstOrDefaultAsync();
    }

    public async Task<EscalationLevel> AddAsync(EscalationLevel level)
    {
        _context.EscalationLevels.Add(level);
        await _context.SaveChangesAsync();
        return level;
    }

    public async Task UpdateAsync(EscalationLevel level)
    {
        level.UpdatedAt = DateTime.UtcNow;
        _context.EscalationLevels.Update(level);
        await _context.SaveChangesAsync();
    }
}
