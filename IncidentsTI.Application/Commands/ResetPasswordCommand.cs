using IncidentsTI.Application.DTOs.Auth;
using MediatR;

namespace IncidentsTI.Application.Commands;

/// <summary>
/// Command to reset a user's password using a valid token.
/// </summary>
public class ResetPasswordCommand : IRequest<ResetPasswordResponseDto>
{
    /// <summary>
    /// The reset token received via email.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// The new password to set.
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;
}
