using MediatR;

namespace IncidentsTI.Application.Features.Users.Commands;

/// <summary>
/// Command for toggling user active status
/// </summary>
public class ToggleUserStatusCommand : IRequest<bool>
{
    public string UserId { get; set; } = string.Empty;
}
