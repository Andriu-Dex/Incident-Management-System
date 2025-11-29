using IncidentsTI.Domain.Entities;

namespace IncidentsTI.Domain.Interfaces;

/// <summary>
/// Repository interface for password reset token operations.
/// </summary>
public interface IPasswordResetTokenRepository
{
    /// <summary>
    /// Creates a new password reset token.
    /// </summary>
    /// <param name="token">The token entity to create.</param>
    /// <returns>The created token with its ID.</returns>
    Task<PasswordResetToken> CreateAsync(PasswordResetToken token);

    /// <summary>
    /// Gets a valid (not expired, not used) token by its hashed value.
    /// </summary>
    /// <param name="hashedToken">The hashed token string.</param>
    /// <returns>The token if found and valid, null otherwise.</returns>
    Task<PasswordResetToken?> GetValidTokenAsync(string hashedToken);

    /// <summary>
    /// Gets the most recent valid token for a user.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <returns>The most recent valid token if exists.</returns>
    Task<PasswordResetToken?> GetValidTokenByUserIdAsync(string userId);

    /// <summary>
    /// Marks a token as used.
    /// </summary>
    /// <param name="token">The token to mark as used.</param>
    Task MarkAsUsedAsync(PasswordResetToken token);

    /// <summary>
    /// Invalidates all existing tokens for a user (when a new one is requested).
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    Task InvalidateAllUserTokensAsync(string userId);

    /// <summary>
    /// Cleans up expired tokens from the database (maintenance operation).
    /// </summary>
    /// <param name="olderThan">Remove tokens older than this date.</param>
    /// <returns>Number of tokens removed.</returns>
    Task<int> CleanupExpiredTokensAsync(DateTime olderThan);
}
