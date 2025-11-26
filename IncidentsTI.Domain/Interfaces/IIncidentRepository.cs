using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Enums;

namespace IncidentsTI.Domain.Interfaces;

public interface IIncidentRepository
{
    Task<Incident?> GetByIdAsync(int id);
    Task<Incident?> GetByTicketNumberAsync(string ticketNumber);
    Task<IEnumerable<Incident>> GetAllAsync();
    Task<IEnumerable<Incident>> GetByUserIdAsync(string userId);
    Task<IEnumerable<Incident>> GetByAssignedToIdAsync(string assignedToId);
    Task<IEnumerable<Incident>> GetByStatusAsync(IncidentStatus status);
    Task<IEnumerable<Incident>> GetByServiceIdAsync(int serviceId);
    Task<IEnumerable<Incident>> GetByPriorityAsync(IncidentPriority priority);
    Task<Incident> AddAsync(Incident incident);
    Task UpdateAsync(Incident incident);
    Task<bool> DeleteAsync(int id);
    Task<string> GetLastTicketNumberAsync();
}
