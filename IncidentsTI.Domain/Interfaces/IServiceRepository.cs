using IncidentsTI.Domain.Entities;

namespace IncidentsTI.Domain.Interfaces;

public interface IServiceRepository
{
    Task<Service?> GetByIdAsync(int id);
    Task<IEnumerable<Service>> GetAllAsync();
    Task<IEnumerable<Service>> GetActiveServicesAsync();
    Task<Service> AddAsync(Service service);
    Task UpdateAsync(Service service);
    Task DeleteAsync(int id);
}
