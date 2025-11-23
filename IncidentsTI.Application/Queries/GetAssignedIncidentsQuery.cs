using IncidentsTI.Application.DTOs;
using MediatR;

namespace IncidentsTI.Application.Queries;

public class GetAssignedIncidentsQuery : IRequest<IEnumerable<IncidentListDto>>
{
    public string AssignedToId { get; set; } = string.Empty;
}
