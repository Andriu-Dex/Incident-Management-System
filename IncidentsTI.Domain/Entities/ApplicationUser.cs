using Microsoft.AspNetCore.Identity;

namespace IncidentsTI.Domain.Entities;

/// <summary>
/// Application user entity extending Identity user
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indica si el usuario desea recibir notificaciones por email
    /// </summary>
    public bool EmailNotificationsEnabled { get; set; } = true;

    /// <summary>
    /// Preferencia de tema del usuario (Light, Dark, Auto)
    /// </summary>
    public string ThemePreference { get; set; } = "Auto";

    // Navigation properties will be added as we develop other phases
}
