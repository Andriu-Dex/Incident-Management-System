using System.Security.Cryptography;
using IncidentsTI.Application.Commands;
using IncidentsTI.Application.DTOs.Auth;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace IncidentsTI.Application.Handlers;

/// <summary>
/// Handler for processing password reset requests.
/// Generates a secure token and (in production) sends it via email.
/// </summary>
public class RequestPasswordResetCommandHandler : IRequestHandler<RequestPasswordResetCommand, ForgotPasswordResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPasswordResetTokenRepository _tokenRepository;

    public RequestPasswordResetCommandHandler(
        UserManager<ApplicationUser> userManager,
        IPasswordResetTokenRepository tokenRepository)
    {
        _userManager = userManager;
        _tokenRepository = tokenRepository;
    }

    public async Task<ForgotPasswordResponseDto> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
    {
        // Security: Always return success message to prevent email enumeration attacks
        var successMessage = "Si el correo electrónico existe en nuestro sistema, recibirá instrucciones para restablecer su contraseña.";

        // Find user by email
        var user = await _userManager.FindByEmailAsync(request.Email);
        
        if (user == null || !user.IsActive)
        {
            // Return success even if user doesn't exist (security measure)
            return new ForgotPasswordResponseDto
            {
                Success = true,
                Message = successMessage
            };
        }

        // Invalidate any existing tokens for this user
        await _tokenRepository.InvalidateAllUserTokensAsync(user.Id);

        // Generate a secure random token
        var plainToken = GenerateSecureToken();
        var hashedToken = HashToken(plainToken);

        // Create the token entity
        var resetToken = PasswordResetToken.Create(user.Id, hashedToken, request.IpAddress);
        await _tokenRepository.CreateAsync(resetToken);

        // Generate the reset link
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(plainToken));
        var resetLink = $"{request.BaseUrl}/reset-password?token={encodedToken}";

        // TODO: In production, send email here instead of returning the link
        // await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink);

        // For development/demo: return the link directly
        // In production, remove ResetLink from response
        return new ForgotPasswordResponseDto
        {
            Success = true,
            Message = successMessage,
            ResetLink = resetLink // Remove this in production!
        };
    }

    /// <summary>
    /// Generates a cryptographically secure random token.
    /// </summary>
    private static string GenerateSecureToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    /// <summary>
    /// Hashes a token for secure storage.
    /// </summary>
    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
