using MediatR;

namespace IncidentsTI.Application.Commands;

public record ToggleServiceStatusCommand(int ServiceId) : IRequest<bool>;
