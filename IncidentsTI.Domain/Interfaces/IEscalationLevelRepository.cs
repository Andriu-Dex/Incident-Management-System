using IncidentsTI.Domain.Entities;

namespace IncidentsTI.Domain.Interfaces;

public interface IEscalationLevelRepository
{
    Task<EscalationLevel?> GetByIdAsync(int id);
    Task<List<EscalationLevel>> GetAllAsync();
    Task<List<EscalationLevel>> GetActiveAsync();
    Task<EscalationLevel?> GetByOrderAsync(int order);
    Task<EscalationLevel?> GetNextLevelAsync(int currentOrder);
    Task<EscalationLevel> AddAsync(EscalationLevel level);
    Task UpdateAsync(EscalationLevel level);
}
