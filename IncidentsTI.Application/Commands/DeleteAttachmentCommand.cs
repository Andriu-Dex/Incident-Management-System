using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Commands;

public class DeleteAttachmentCommand : IRequest<bool>
{
    public int AttachmentId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
}

public class DeleteAttachmentCommandHandler : IRequestHandler<DeleteAttachmentCommand, bool>
{
    private readonly IIncidentAttachmentRepository _attachmentRepository;

    public DeleteAttachmentCommandHandler(IIncidentAttachmentRepository attachmentRepository)
    {
        _attachmentRepository = attachmentRepository;
    }

    public async Task<bool> Handle(DeleteAttachmentCommand request, CancellationToken cancellationToken)
    {
        var attachment = await _attachmentRepository.GetByIdAsync(request.AttachmentId);
        if (attachment == null)
            return false;

        // Solo el usuario que subió el archivo o un admin puede eliminarlo
        if (!request.IsAdmin && attachment.UploadedById != request.UserId)
            throw new UnauthorizedAccessException("No tiene permiso para eliminar este archivo");

        await _attachmentRepository.DeleteAsync(request.AttachmentId);
        return true;
    }
}
