using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Interfaces;
using IncidentsTI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IncidentsTI.Infrastructure.Repositories;

public class IncidentAttachmentRepository : IIncidentAttachmentRepository
{
    private readonly ApplicationDbContext _context;

    public IncidentAttachmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IncidentAttachment?> GetByIdAsync(int id)
    {
        return await _context.IncidentAttachments
            .Include(a => a.UploadedBy)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<IncidentAttachment>> GetByIncidentIdAsync(int incidentId)
    {
        return await _context.IncidentAttachments
            .Include(a => a.UploadedBy)
            .Where(a => a.IncidentId == incidentId)
            .OrderByDescending(a => a.UploadedAt)
            .ToListAsync();
    }

    public async Task<IncidentAttachment> AddAsync(IncidentAttachment attachment)
    {
        _context.IncidentAttachments.Add(attachment);
        await _context.SaveChangesAsync();
        return attachment;
    }

    public async Task DeleteAsync(int id)
    {
        var attachment = await _context.IncidentAttachments.FindAsync(id);
        if (attachment != null)
        {
            _context.IncidentAttachments.Remove(attachment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetAttachmentCountByIncidentAsync(int incidentId)
    {
        return await _context.IncidentAttachments
            .CountAsync(a => a.IncidentId == incidentId);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.IncidentAttachments.AnyAsync(a => a.Id == id);
    }
}
