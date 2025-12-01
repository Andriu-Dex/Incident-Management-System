using MediatR;

namespace IncidentsTI.Application.Features.Users.Commands;

/// <summary>
/// Command for updating a user's role.
/// Only one role per user is supported.
/// </summary>
public class UpdateUserRoleCommand : IRequest<bool>
{
    /// <summary>
    /// The ID of the user whose role will be updated.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The new role to assign to the user.
    /// </summary>
    public string NewRole { get; set; } = string.Empty;
}
