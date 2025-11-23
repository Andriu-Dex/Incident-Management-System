using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Enums;
using IncidentsTI.Domain.Interfaces;
using IncidentsTI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IncidentsTI.Infrastructure.Repositories;

public class IncidentRepository : IIncidentRepository
{
    private readonly ApplicationDbContext _context;

    public IncidentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Incident?> GetByIdAsync(int id)
    {
        return await _context.Incidents
            .Include(i => i.User)
            .Include(i => i.Service)
            .Include(i => i.AssignedTo)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Incident?> GetByTicketNumberAsync(string ticketNumber)
    {
        return await _context.Incidents
            .Include(i => i.User)
            .Include(i => i.Service)
            .Include(i => i.AssignedTo)
            .FirstOrDefaultAsync(i => i.TicketNumber == ticketNumber);
    }

    public async Task<IEnumerable<Incident>> GetAllAsync()
    {
        return await _context.Incidents
            .Include(i => i.User)
            .Include(i => i.Service)
            .Include(i => i.AssignedTo)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Incident>> GetByUserIdAsync(string userId)
    {
        return await _context.Incidents
            .Include(i => i.User)
            .Include(i => i.Service)
            .Include(i => i.AssignedTo)
            .Where(i => i.UserId == userId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Incident>> GetByAssignedToIdAsync(string assignedToId)
    {
        return await _context.Incidents
            .Include(i => i.User)
            .Include(i => i.Service)
            .Include(i => i.AssignedTo)
            .Where(i => i.AssignedToId == assignedToId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Incident>> GetByStatusAsync(IncidentStatus status)
    {
        return await _context.Incidents
            .Include(i => i.User)
            .Include(i => i.Service)
            .Include(i => i.AssignedTo)
            .Where(i => i.Status == status)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Incident>> GetByServiceIdAsync(int serviceId)
    {
        return await _context.Incidents
            .Include(i => i.User)
            .Include(i => i.Service)
            .Include(i => i.AssignedTo)
            .Where(i => i.ServiceId == serviceId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Incident>> GetByPriorityAsync(IncidentPriority priority)
    {
        return await _context.Incidents
            .Include(i => i.User)
            .Include(i => i.Service)
            .Include(i => i.AssignedTo)
            .Where(i => i.Priority == priority)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<Incident> AddAsync(Incident incident)
    {
        _context.Incidents.Add(incident);
        await _context.SaveChangesAsync();
        
        // Recargar con relaciones
        return await GetByIdAsync(incident.Id) ?? incident;
    }

    public async Task UpdateAsync(Incident incident)
    {
        _context.Incidents.Update(incident);
        await _context.SaveChangesAsync();
    }

    public async Task<string> GetLastTicketNumberAsync()
    {
        var lastIncident = await _context.Incidents
            .OrderByDescending(i => i.TicketNumber)
            .FirstOrDefaultAsync();

        return lastIncident?.TicketNumber ?? string.Empty;
    }
}
