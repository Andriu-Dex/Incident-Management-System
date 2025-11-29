using IncidentsTI.Application.DTOs.Auth;
using MediatR;

namespace IncidentsTI.Application.Commands;

/// <summary>
/// Command to validate a password reset token.
/// </summary>
public class ValidateResetTokenCommand : IRequest<ValidateResetTokenResponseDto>
{
    /// <summary>
    /// The token to validate.
    /// </summary>
    public string Token { get; set; } = string.Empty;
}
