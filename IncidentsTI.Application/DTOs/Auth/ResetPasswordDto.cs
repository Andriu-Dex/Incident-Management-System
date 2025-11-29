using System.ComponentModel.DataAnnotations;

namespace IncidentsTI.Application.DTOs.Auth;

/// <summary>
/// DTO for validating a password reset token.
/// </summary>
public class ValidateResetTokenDto
{
    /// <summary>
    /// The reset token to validate.
    /// </summary>
    [Required(ErrorMessage = "El token es requerido")]
    public string Token { get; set; } = string.Empty;
}

/// <summary>
/// Response for token validation.
/// </summary>
public class ValidateResetTokenResponseDto
{
    /// <summary>
    /// Whether the token is valid.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// The user's email (masked for privacy).
    /// </summary>
    public string? MaskedEmail { get; set; }

    /// <summary>
    /// Error message if token is invalid.
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// DTO for resetting the password with a valid token.
/// </summary>
public class ResetPasswordDto
{
    /// <summary>
    /// The reset token received via email.
    /// </summary>
    [Required(ErrorMessage = "El token es requerido")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// The new password.
    /// </summary>
    [Required(ErrorMessage = "La nueva contraseña es requerida")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "La contraseña debe contener al menos una mayúscula, una minúscula, un número y un carácter especial")]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Confirmation of the new password.
    /// </summary>
    [Required(ErrorMessage = "Debe confirmar la contraseña")]
    [Compare(nameof(NewPassword), ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// Response for password reset operation.
/// </summary>
public class ResetPasswordResponseDto
{
    /// <summary>
    /// Whether the password was reset successfully.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Message to display to the user.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
