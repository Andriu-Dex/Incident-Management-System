using IncidentsTI.Application.Commands;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Enums;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, int>
{
    private readonly IIncidentCommentRepository _commentRepository;
    private readonly IIncidentHistoryRepository _historyRepository;
    private readonly IIncidentRepository _incidentRepository;

    public AddCommentCommandHandler(
        IIncidentCommentRepository commentRepository,
        IIncidentHistoryRepository historyRepository,
        IIncidentRepository incidentRepository)
    {
        _commentRepository = commentRepository;
        _historyRepository = historyRepository;
        _incidentRepository = incidentRepository;
    }

    public async Task<int> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        // Verificar que el incidente existe
        var incident = await _incidentRepository.GetByIdAsync(request.IncidentId);
        if (incident == null)
            return 0;

        // Crear el comentario
        var comment = new IncidentComment
        {
            IncidentId = request.IncidentId,
            UserId = request.UserId,
            Content = request.Content,
            IsInternal = request.IsInternal,
            CreatedAt = DateTime.UtcNow
        };

        await _commentRepository.AddAsync(comment);

        // Registrar en el historial
        var history = new IncidentHistory
        {
            IncidentId = request.IncidentId,
            UserId = request.UserId,
            Action = HistoryAction.CommentAdded,
            NewValue = request.IsInternal ? "Comentario interno" : "Comentario público",
            Description = request.Content.Length > 100 
                ? request.Content.Substring(0, 100) + "..." 
                : request.Content,
            Timestamp = DateTime.UtcNow
        };

        await _historyRepository.AddAsync(history);

        return comment.Id;
    }
}
