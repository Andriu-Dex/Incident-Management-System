using IncidentsTI.Application.DTOs.Escalation;
using IncidentsTI.Application.Queries;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

public class GetIncidentEscalationHistoryQueryHandler : IRequestHandler<GetIncidentEscalationHistoryQuery, List<IncidentEscalationDto>>
{
    private readonly IIncidentEscalationRepository _repository;

    public GetIncidentEscalationHistoryQueryHandler(IIncidentEscalationRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<IncidentEscalationDto>> Handle(GetIncidentEscalationHistoryQuery request, CancellationToken cancellationToken)
    {
        var escalations = await _repository.GetByIncidentIdAsync(request.IncidentId);

        return escalations.Select(e => new IncidentEscalationDto
        {
            Id = e.Id,
            IncidentId = e.IncidentId,
            FromUserId = e.FromUserId,
            FromUserName = e.FromUser != null ? $"{e.FromUser.FirstName} {e.FromUser.LastName}" : "",
            ToUserId = e.ToUserId,
            ToUserName = e.ToUser != null ? $"{e.ToUser.FirstName} {e.ToUser.LastName}" : null,
            FromLevelId = e.FromLevelId,
            FromLevelName = e.FromLevel?.Name,
            ToLevelId = e.ToLevelId,
            ToLevelName = e.ToLevel?.Name ?? "",
            Reason = e.Reason,
            Notes = e.Notes,
            EscalatedAt = e.EscalatedAt,
            FormattedDescription = e.FormattedDescription
        }).ToList();
    }
}
