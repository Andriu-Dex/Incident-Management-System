using IncidentsTI.Domain.Entities;

namespace IncidentsTI.Domain.Interfaces;

/// <summary>
/// Repository interface for user operations
/// </summary>
public interface IUserRepository
{
    Task<ApplicationUser?> GetByIdAsync(string id);
    Task<ApplicationUser?> GetByEmailAsync(string email);
    Task<IEnumerable<ApplicationUser>> GetAllAsync();
    Task<IEnumerable<ApplicationUser>> GetActiveUsersAsync();
    Task<ApplicationUser> CreateAsync(ApplicationUser user, string password);
    Task<bool> UpdateAsync(ApplicationUser user);
    Task<bool> ToggleActiveStatusAsync(string userId);
    Task<bool> DeleteAsync(string userId);
    Task<bool> AddToRoleAsync(ApplicationUser user, string roleName);
    Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
}
