using MediatR;

namespace IncidentsTI.Application.Queries;

public record GetUnreadNotificationCountQuery(string UserId) : IRequest<int>;
