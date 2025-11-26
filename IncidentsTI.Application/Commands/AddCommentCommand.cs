using MediatR;

namespace IncidentsTI.Application.Commands;

/// <summary>
/// Comando para agregar un comentario a un incidente
/// </summary>
public class AddCommentCommand : IRequest<int>
{
    public int IncidentId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsInternal { get; set; } = false;
}
