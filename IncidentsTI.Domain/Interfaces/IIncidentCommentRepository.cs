using IncidentsTI.Domain.Entities;

namespace IncidentsTI.Domain.Interfaces;

public interface IIncidentCommentRepository
{
    /// <summary>
    /// Agrega un nuevo comentario
    /// </summary>
    Task AddAsync(IncidentComment comment);
    
    /// <summary>
    /// Obtiene todos los comentarios de un incidente (para personal de TI)
    /// </summary>
    Task<IEnumerable<IncidentComment>> GetByIncidentIdAsync(int incidentId);
    
    /// <summary>
    /// Obtiene solo los comentarios públicos de un incidente (para usuarios normales)
    /// </summary>
    Task<IEnumerable<IncidentComment>> GetPublicByIncidentIdAsync(int incidentId);
    
    /// <summary>
    /// Obtiene un comentario por su ID
    /// </summary>
    Task<IncidentComment?> GetByIdAsync(int id);
}
