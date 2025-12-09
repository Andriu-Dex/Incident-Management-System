namespace IncidentsTI.Application.DTOs;

public class AttachmentDto
{
    public int Id { get; set; }
    public int IncidentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FormattedSize { get; set; } = string.Empty;
    public bool IsImage { get; set; }
    public string UploadedById { get; set; } = string.Empty;
    public string UploadedByName { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}
