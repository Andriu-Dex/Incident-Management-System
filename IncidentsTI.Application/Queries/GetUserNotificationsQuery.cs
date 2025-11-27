using IncidentsTI.Application.DTOs.Notifications;
using MediatR;

namespace IncidentsTI.Application.Queries;

public record GetUserNotificationsQuery(string UserId, int PageSize = 20) : IRequest<List<NotificationDto>>;
