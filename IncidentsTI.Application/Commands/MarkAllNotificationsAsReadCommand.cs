using MediatR;

namespace IncidentsTI.Application.Commands;

public record MarkAllNotificationsAsReadCommand(string UserId) : IRequest<bool>;
