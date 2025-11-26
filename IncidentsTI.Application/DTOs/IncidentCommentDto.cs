namespace IncidentsTI.Application.DTOs;

/// <summary>
/// DTO para mostrar comentarios de un incidente
/// </summary>
public class IncidentCommentDto
{
    public int Id { get; set; }
    public int IncidentId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsInternal { get; set; }
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Indica si el comentario fue hecho por personal de TI
    /// </summary>
    public bool IsFromStaff => UserRole == "Technician" || UserRole == "Administrator";
}
