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
/// Handler for resetting a user's password with a valid token.
/// </summary>
public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResponseDto>
{
    private readonly IPasswordResetTokenRepository _tokenRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public ResetPasswordCommandHandler(
        IPasswordResetTokenRepository tokenRepository,
        UserManager<ApplicationUser> userManager)
    {
        _tokenRepository = tokenRepository;
        _userManager = userManager;
    }

    public async Task<ResetPasswordResponseDto> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
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
                return new ResetPasswordResponseDto
                {
                    Success = false,
                    Message = "El enlace de recuperación no es válido o ha expirado. Por favor, solicite uno nuevo."
                };
            }

            // Get the user
            var user = await _userManager.FindByIdAsync(resetToken.UserId);
            if (user == null || !user.IsActive)
            {
                return new ResetPasswordResponseDto
                {
                    Success = false,
                    Message = "La cuenta de usuario no está disponible."
                };
            }

            // Generate password reset token from Identity (required by UserManager)
            var identityToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            
            // Reset the password
            var result = await _userManager.ResetPasswordAsync(user, identityToken, request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new ResetPasswordResponseDto
                {
                    Success = false,
                    Message = $"Error al cambiar la contraseña: {errors}"
                };
            }

            // Mark the token as used
            await _tokenRepository.MarkAsUsedAsync(resetToken);

            // Invalidate all other tokens for this user
            await _tokenRepository.InvalidateAllUserTokensAsync(user.Id);

            return new ResetPasswordResponseDto
            {
                Success = true,
                Message = "Su contraseña ha sido cambiada exitosamente. Ahora puede iniciar sesión con su nueva contraseña."
            };
        }
        catch (Exception)
        {
            return new ResetPasswordResponseDto
            {
                Success = false,
                Message = "Ocurrió un error al procesar su solicitud. Por favor, intente nuevamente."
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
}
