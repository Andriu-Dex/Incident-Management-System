using MediatR;

namespace IncidentsTI.Application.Commands;

public record MarkNotificationAsReadCommand(int NotificationId) : IRequest<bool>;
