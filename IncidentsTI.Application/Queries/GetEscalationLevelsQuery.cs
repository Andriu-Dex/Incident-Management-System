using IncidentsTI.Application.DTOs.Escalation;
using MediatR;

namespace IncidentsTI.Application.Queries;

public class GetEscalationLevelsQuery : IRequest<List<EscalationLevelDto>>
{
    public bool OnlyActive { get; set; } = true;
}
