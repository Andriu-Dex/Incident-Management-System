using IncidentsTI.Application.DTOs;
using MediatR;

namespace IncidentsTI.Application.Queries;

/// <summary>
/// Query para obtener el historial completo de un incidente
/// </summary>
public class GetIncidentHistoryQuery : IRequest<IEnumerable<IncidentHistoryDto>>
{
    public int IncidentId { get; set; }
    
    public GetIncidentHistoryQuery(int incidentId)
    {
        IncidentId = incidentId;
    }
}
