using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Interfaces;
using IncidentsTI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IncidentsTI.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for password reset token operations.
/// </summary>
public class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly ApplicationDbContext _context;

    public PasswordResetTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<PasswordResetToken> CreateAsync(PasswordResetToken token)
    {
        _context.PasswordResetTokens.Add(token);
        await _context.SaveChangesAsync();
        return token;
    }

    /// <inheritdoc />
    public async Task<PasswordResetToken?> GetValidTokenAsync(string hashedToken)
    {
        return await _context.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => 
                t.Token == hashedToken && 
                !t.IsUsed && 
                t.ExpiresAt > DateTime.UtcNow);
    }

    /// <inheritdoc />
    public async Task<PasswordResetToken?> GetValidTokenByUserIdAsync(string userId)
    {
        return await _context.PasswordResetTokens
            .Where(t => t.UserId == userId && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task MarkAsUsedAsync(PasswordResetToken token)
    {
        token.MarkAsUsed();
        _context.PasswordResetTokens.Update(token);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task InvalidateAllUserTokensAsync(string userId)
    {
        var tokens = await _context.PasswordResetTokens
            .Where(t => t.UserId == userId && !t.IsUsed)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.MarkAsUsed();
        }

        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<int> CleanupExpiredTokensAsync(DateTime olderThan)
    {
        var expiredTokens = await _context.PasswordResetTokens
            .Where(t => t.ExpiresAt < olderThan || t.IsUsed)
            .ToListAsync();

        _context.PasswordResetTokens.RemoveRange(expiredTokens);
        await _context.SaveChangesAsync();

        return expiredTokens.Count;
    }
}
