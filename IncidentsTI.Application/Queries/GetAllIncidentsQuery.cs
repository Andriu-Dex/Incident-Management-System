using IncidentsTI.Application.DTOs;
using MediatR;

namespace IncidentsTI.Application.Queries;

public class GetAllIncidentsQuery : IRequest<IEnumerable<IncidentListDto>>
{
    public IncidentsTI.Domain.Enums.IncidentStatus? StatusFilter { get; set; }
    public int? ServiceIdFilter { get; set; }
    public IncidentsTI.Domain.Enums.IncidentPriority? PriorityFilter { get; set; }
}
