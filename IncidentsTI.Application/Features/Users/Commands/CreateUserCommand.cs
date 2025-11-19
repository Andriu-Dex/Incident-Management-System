using IncidentsTI.Application.DTOs.Users;
using MediatR;

namespace IncidentsTI.Application.Features.Users.Commands;

/// <summary>
/// Command for creating a new user
/// </summary>
public class CreateUserCommand : IRequest<UserDto>
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
