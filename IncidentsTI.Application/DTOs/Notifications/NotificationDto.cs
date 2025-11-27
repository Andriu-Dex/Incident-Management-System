namespace IncidentsTI.Application.DTOs.Notifications;

public class NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string TypeIcon { get; set; } = string.Empty;
    public string TypeClass { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public string? ActionUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
