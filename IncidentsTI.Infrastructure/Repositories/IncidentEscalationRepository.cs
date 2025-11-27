using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Interfaces;
using IncidentsTI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IncidentsTI.Infrastructure.Repositories;

public class IncidentEscalationRepository : IIncidentEscalationRepository
{
    private readonly ApplicationDbContext _context;

    public IncidentEscalationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IncidentEscalation?> GetByIdAsync(int id)
    {
        return await _context.IncidentEscalations
            .Include(e => e.FromUser)
            .Include(e => e.ToUser)
            .Include(e => e.FromLevel)
            .Include(e => e.ToLevel)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<IncidentEscalation>> GetByIncidentIdAsync(int incidentId)
    {
        return await _context.IncidentEscalations
            .Include(e => e.FromUser)
            .Include(e => e.ToUser)
            .Include(e => e.FromLevel)
            .Include(e => e.ToLevel)
            .Where(e => e.IncidentId == incidentId)
            .OrderByDescending(e => e.EscalatedAt)
            .ToListAsync();
    }

    public async Task<IncidentEscalation?> GetLatestByIncidentIdAsync(int incidentId)
    {
        return await _context.IncidentEscalations
            .Include(e => e.FromUser)
            .Include(e => e.ToUser)
            .Include(e => e.FromLevel)
            .Include(e => e.ToLevel)
            .Where(e => e.IncidentId == incidentId)
            .OrderByDescending(e => e.EscalatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<IncidentEscalation> AddAsync(IncidentEscalation escalation)
    {
        _context.IncidentEscalations.Add(escalation);
        await _context.SaveChangesAsync();
        return escalation;
    }
}
