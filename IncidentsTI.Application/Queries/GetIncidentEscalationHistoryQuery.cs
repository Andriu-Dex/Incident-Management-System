using IncidentsTI.Application.DTOs.Escalation;
using MediatR;

namespace IncidentsTI.Application.Queries;

public class GetIncidentEscalationHistoryQuery : IRequest<List<IncidentEscalationDto>>
{
    public int IncidentId { get; set; }
}
