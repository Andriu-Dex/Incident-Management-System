using IncidentsTI.Application.DTOs;
using IncidentsTI.Application.Queries;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

public class GetIncidentHistoryQueryHandler : IRequestHandler<GetIncidentHistoryQuery, IEnumerable<IncidentHistoryDto>>
{
    private readonly IIncidentHistoryRepository _historyRepository;

    public GetIncidentHistoryQueryHandler(IIncidentHistoryRepository historyRepository)
    {
        _historyRepository = historyRepository;
    }

    public async Task<IEnumerable<IncidentHistoryDto>> Handle(GetIncidentHistoryQuery request, CancellationToken cancellationToken)
    {
        var history = await _historyRepository.GetByIncidentIdAsync(request.IncidentId);

        return history.Select(h => new IncidentHistoryDto
        {
            Id = h.Id,
            IncidentId = h.IncidentId,
            UserId = h.UserId,
            UserName = $"{h.User.FirstName} {h.User.LastName}",
            Action = h.Action,
            OldValue = h.OldValue,
            NewValue = h.NewValue,
            Description = h.Description,
            Timestamp = h.Timestamp
        });
    }
}
