using System.ComponentModel.DataAnnotations;

namespace IncidentsTI.Application.DTOs.Auth;

/// <summary>
/// DTO for requesting a password reset.
/// </summary>
public class ForgotPasswordDto
{
    /// <summary>
    /// The email address of the user requesting password reset.
    /// </summary>
    [Required(ErrorMessage = "El correo electrónico es requerido")]
    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO for password reset request.
/// </summary>
public class ForgotPasswordResponseDto
{
    /// <summary>
    /// Whether the request was processed successfully.
    /// Note: This will be true even if the email doesn't exist (security measure).
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Message to display to the user.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// For development/demo purposes only - the reset link.
    /// In production, this would be sent via email.
    /// </summary>
    public string? ResetLink { get; set; }
}
