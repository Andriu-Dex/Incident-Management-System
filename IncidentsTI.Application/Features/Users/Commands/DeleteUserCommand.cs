using MediatR;

namespace IncidentsTI.Application.Features.Users.Commands;

/// <summary>
/// Command for deleting a user
/// </summary>
public class DeleteUserCommand : IRequest<bool>
{
    public string UserId { get; set; } = string.Empty;
}
