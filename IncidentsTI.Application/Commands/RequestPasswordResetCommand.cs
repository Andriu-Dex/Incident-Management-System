using IncidentsTI.Application.DTOs.Auth;
using MediatR;

namespace IncidentsTI.Application.Commands;

/// <summary>
/// Command to request a password reset for a user account.
/// </summary>
public class RequestPasswordResetCommand : IRequest<ForgotPasswordResponseDto>
{
    /// <summary>
    /// The email address of the user requesting password reset.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The base URL for generating the reset link.
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// The IP address from which the request originated (for audit).
    /// </summary>
    public string? IpAddress { get; set; }
}
