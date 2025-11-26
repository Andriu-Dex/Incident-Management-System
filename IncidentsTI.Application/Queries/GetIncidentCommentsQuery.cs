using IncidentsTI.Application.DTOs;
using MediatR;

namespace IncidentsTI.Application.Queries;

/// <summary>
/// Query para obtener los comentarios de un incidente
/// </summary>
public class GetIncidentCommentsQuery : IRequest<IEnumerable<IncidentCommentDto>>
{
    public int IncidentId { get; set; }
    
    /// <summary>
    /// Si es true, retorna todos los comentarios (incluyendo internos)
    /// Si es false, solo retorna comentarios públicos
    /// </summary>
    public bool IncludeInternal { get; set; }
    
    public GetIncidentCommentsQuery(int incidentId, bool includeInternal = false)
    {
        IncidentId = incidentId;
        IncludeInternal = includeInternal;
    }
}
