using IncidentsTI.Domain.Entities;

namespace IncidentsTI.Domain.Interfaces;

public interface IIncidentAttachmentRepository
{
    Task<IncidentAttachment?> GetByIdAsync(int id);
    Task<IEnumerable<IncidentAttachment>> GetByIncidentIdAsync(int incidentId);
    Task<IncidentAttachment> AddAsync(IncidentAttachment attachment);
    Task DeleteAsync(int id);
    Task<int> GetAttachmentCountByIncidentAsync(int incidentId);
    Task<bool> ExistsAsync(int id);
}
