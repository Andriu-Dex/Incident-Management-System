using IncidentsTI.Domain.Entities;

namespace IncidentsTI.Domain.Interfaces;

public interface IIncidentHistoryRepository
{
    /// <summary>
    /// Agrega un nuevo registro de historial
    /// </summary>
    Task AddAsync(IncidentHistory history);
    
    /// <summary>
    /// Obtiene todo el historial de un incidente ordenado por fecha descendente
    /// </summary>
    Task<IEnumerable<IncidentHistory>> GetByIncidentIdAsync(int incidentId);
    
    /// <summary>
    /// Obtiene las últimas N acciones de un incidente
    /// </summary>
    Task<IEnumerable<IncidentHistory>> GetRecentByIncidentIdAsync(int incidentId, int count = 10);
}
