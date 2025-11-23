using IncidentsTI.Application.DTOs;
using MediatR;

namespace IncidentsTI.Application.Queries;

public class GetUserIncidentsQuery : IRequest<IEnumerable<IncidentListDto>>
{
    public string UserId { get; set; } = string.Empty;
}
