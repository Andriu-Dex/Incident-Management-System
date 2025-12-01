using IncidentsTI.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IncidentsTI.Application.Features.Users.Commands;

/// <summary>
/// Handler for updating a user's role.
/// Removes all existing roles and assigns the new one.
/// </summary>
public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UpdateUserRoleCommandHandler> _logger;

    // Valid roles in the system
    private static readonly HashSet<string> ValidRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Student",
        "Teacher", 
        "Administrative",
        "Technician",
        "Administrator"
    };

    public UpdateUserRoleCommandHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<UpdateUserRoleCommandHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate role
            if (!ValidRoles.Contains(request.NewRole))
            {
                _logger.LogWarning("Attempted to assign invalid role: {Role}", request.NewRole);
                return false;
            }

            // Find user
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", request.UserId);
                return false;
            }

            // Get current roles
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Check if user already has this role
            if (currentRoles.Contains(request.NewRole))
            {
                _logger.LogInformation("User {UserId} already has role {Role}", request.UserId, request.NewRole);
                return true; // Already has the role, consider it a success
            }

            // Remove all current roles
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    _logger.LogError("Failed to remove roles from user {UserId}: {Errors}", 
                        request.UserId, 
                        string.Join(", ", removeResult.Errors.Select(e => e.Description)));
                    return false;
                }
            }

            // Add new role
            var addResult = await _userManager.AddToRoleAsync(user, request.NewRole);
            if (!addResult.Succeeded)
            {
                _logger.LogError("Failed to add role {Role} to user {UserId}: {Errors}", 
                    request.NewRole,
                    request.UserId, 
                    string.Join(", ", addResult.Errors.Select(e => e.Description)));
                return false;
            }

            // Update the UpdatedAt timestamp
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("Successfully changed role for user {UserId} from [{OldRoles}] to {NewRole}", 
                request.UserId, 
                string.Join(", ", currentRoles),
                request.NewRole);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role for user {UserId}", request.UserId);
            return false;
        }
    }
}
