using System.Security.Cryptography;
using System.Text;
using IncidentsTI.Application.Commands;
using IncidentsTI.Application.DTOs.Auth;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace IncidentsTI.Application.Handlers;

/// <summary>
/// Handler for validating password reset tokens.
/// </summary>
public class ValidateResetTokenCommandHandler : IRequestHandler<ValidateResetTokenCommand, ValidateResetTokenResponseDto>
{
    private readonly IPasswordResetTokenRepository _tokenRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public ValidateResetTokenCommandHandler(
        IPasswordResetTokenRepository tokenRepository,
        UserManager<ApplicationUser> userManager)
    {
        _tokenRepository = tokenRepository;
        _userManager = userManager;
    }

    public async Task<ValidateResetTokenResponseDto> Handle(ValidateResetTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Decode the token from URL-safe base64
            var decodedBytes = WebEncoders.Base64UrlDecode(request.Token);
            var plainToken = Encoding.UTF8.GetString(decodedBytes);
            var hashedToken = HashToken(plainToken);

            // Look for a valid token
            var resetToken = await _tokenRepository.GetValidTokenAsync(hashedToken);

            if (resetToken == null)
            {
                return new ValidateResetTokenResponseDto
                {
                    IsValid = false,
                    ErrorMessage = "El enlace de recuperación no es válido o ha expirado."
                };
            }

            // Get user info for display
            var user = await _userManager.FindByIdAsync(resetToken.UserId);
            if (user == null || !user.IsActive)
            {
                return new ValidateResetTokenResponseDto
                {
                    IsValid = false,
                    ErrorMessage = "La cuenta de usuario no está disponible."
                };
            }

            // Mask the email for privacy (e.g., "u***@uta.edu.ec")
            var maskedEmail = MaskEmail(user.Email!);

            return new ValidateResetTokenResponseDto
            {
                IsValid = true,
                MaskedEmail = maskedEmail
            };
        }
        catch (Exception)
        {
            return new ValidateResetTokenResponseDto
            {
                IsValid = false,
                ErrorMessage = "El enlace de recuperación no es válido."
            };
        }
    }

    /// <summary>
    /// Hashes a token for comparison with stored hash.
    /// </summary>
    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Masks an email address for privacy display.
    /// Example: "usuario@uta.edu.ec" becomes "u***@uta.edu.ec"
    /// </summary>
    private static string MaskEmail(string email)
    {
        var parts = email.Split('@');
        if (parts.Length != 2) return "***@***.***";

        var localPart = parts[0];
        var domain = parts[1];

        var maskedLocal = localPart.Length > 2
            ? localPart[0] + new string('*', Math.Min(localPart.Length - 1, 5)) + localPart[^1]
            : localPart[0] + "***";

        return $"{maskedLocal}@{domain}";
    }
}
