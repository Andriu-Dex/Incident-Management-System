using MediatR;

namespace IncidentsTI.Application.Commands;

public record DeleteServiceCommand(int ServiceId) : IRequest<bool>;
