using IncidentsTI.Application.DTOs;
using MediatR;

namespace IncidentsTI.Application.Queries;

public class GetIncidentByIdQuery : IRequest<IncidentDto?>
{
    public int Id { get; set; }
}
