using MediatR;

namespace IncidentsTI.Application.Commands;

public record DeleteNotificationCommand(int NotificationId) : IRequest<bool>;
