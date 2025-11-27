using IncidentsTI.Domain.Entities;

namespace IncidentsTI.Domain.Interfaces;

public interface IIncidentEscalationRepository
{
    Task<IncidentEscalation?> GetByIdAsync(int id);
    Task<List<IncidentEscalation>> GetByIncidentIdAsync(int incidentId);
    Task<IncidentEscalation?> GetLatestByIncidentIdAsync(int incidentId);
    Task<IncidentEscalation> AddAsync(IncidentEscalation escalation);
}
