using IncidentsTI.Application.DTOs.Auth;
using MediatR;

namespace IncidentsTI.Application.Features.Auth.Commands;

/// <summary>
/// Command for user login
/// </summary>
public class LoginCommand : IRequest<AuthResponseDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}
