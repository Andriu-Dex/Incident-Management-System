using IncidentsTI.Domain.Entities;

namespace IncidentsTI.Domain.Entities;

/// <summary>
/// Entity representing a password reset token for user account recovery.
/// Tokens are single-use and have an expiration time for security.
/// </summary>
public class PasswordResetToken
{
    /// <summary>
    /// Unique identifier for the token record.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The user ID associated with this password reset request.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The secure token string used to validate the reset request.
    /// This is a hashed value for security.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// The token that will be sent to the user (unhashed, for email).
    /// This is NOT stored in the database - only used transiently.
    /// </summary>
    public string? PlainToken { get; set; }

    /// <summary>
    /// When this token was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this token expires. Default is 1 hour from creation.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Whether this token has been used to reset the password.
    /// Once used, a token cannot be reused.
    /// </summary>
    public bool IsUsed { get; set; } = false;

    /// <summary>
    /// When this token was used (if applicable).
    /// </summary>
    public DateTime? UsedAt { get; set; }

    /// <summary>
    /// IP address from which the reset was requested (for audit purposes).
    /// </summary>
    public string? RequestedFromIp { get; set; }

    /// <summary>
    /// Navigation property to the user.
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// Checks if the token is still valid (not expired and not used).
    /// </summary>
    public bool IsValid => !IsUsed && DateTime.UtcNow < ExpiresAt;

    /// <summary>
    /// Creates a new password reset token with default expiration of 1 hour.
    /// </summary>
    public static PasswordResetToken Create(string userId, string hashedToken, string? ipAddress = null)
    {
        return new PasswordResetToken
        {
            UserId = userId,
            Token = hashedToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            IsUsed = false,
            RequestedFromIp = ipAddress
        };
    }

    /// <summary>
    /// Marks the token as used.
    /// </summary>
    public void MarkAsUsed()
    {
        IsUsed = true;
        UsedAt = DateTime.UtcNow;
    }
}
