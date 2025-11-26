namespace IncidentsTI.Application.DTOs;

/// <summary>
/// DTO para crear un nuevo comentario
/// </summary>
public class CreateCommentDto
{
    public int IncidentId { get; set; }
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Si es true, el comentario solo será visible para personal de TI
    /// </summary>
    public bool IsInternal { get; set; } = false;
}
