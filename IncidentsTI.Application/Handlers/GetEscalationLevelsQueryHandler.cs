using IncidentsTI.Application.DTOs.Escalation;
using IncidentsTI.Application.Queries;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

public class GetEscalationLevelsQueryHandler : IRequestHandler<GetEscalationLevelsQuery, List<EscalationLevelDto>>
{
    private readonly IEscalationLevelRepository _repository;

    public GetEscalationLevelsQueryHandler(IEscalationLevelRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<EscalationLevelDto>> Handle(GetEscalationLevelsQuery request, CancellationToken cancellationToken)
    {
        var levels = request.OnlyActive 
            ? await _repository.GetActiveAsync()
            : await _repository.GetAllAsync();

        return levels.Select(l => new EscalationLevelDto
        {
            Id = l.Id,
            Name = l.Name,
            Description = l.Description,
            Order = l.Order,
            IsActive = l.IsActive
        }).ToList();
    }
}
