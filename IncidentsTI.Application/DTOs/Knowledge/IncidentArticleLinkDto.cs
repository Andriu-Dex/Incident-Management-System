namespace IncidentsTI.Application.DTOs.Knowledge;

/// <summary>
/// DTO para el vínculo entre incidente y artículo
/// </summary>
public class IncidentArticleLinkDto
{
    public int Id { get; set; }
    public int IncidentId { get; set; }
    public string IncidentTicket { get; set; } = string.Empty;
    public int ArticleId { get; set; }
    public string ArticleTitle { get; set; } = string.Empty;
    public string LinkedByUserName { get; set; } = string.Empty;
    public DateTime LinkedAt { get; set; }
    public bool WasHelpful { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// DTO para crear un vínculo entre incidente y artículo
/// </summary>
public class CreateArticleLinkDto
{
    public int IncidentId { get; set; }
    public int ArticleId { get; set; }
    public bool WasHelpful { get; set; } = true;
    public string? Notes { get; set; }
}
