using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Queries;

public class GetAttachmentContentQuery : IRequest<(byte[] Content, string ContentType, string FileName)?>
{
    public int AttachmentId { get; set; }
}

public class GetAttachmentContentQueryHandler : IRequestHandler<GetAttachmentContentQuery, (byte[] Content, string ContentType, string FileName)?>
{
    private readonly IIncidentAttachmentRepository _attachmentRepository;

    public GetAttachmentContentQueryHandler(IIncidentAttachmentRepository attachmentRepository)
    {
        _attachmentRepository = attachmentRepository;
    }

    public async Task<(byte[] Content, string ContentType, string FileName)?> Handle(GetAttachmentContentQuery request, CancellationToken cancellationToken)
    {
        var attachment = await _attachmentRepository.GetByIdAsync(request.AttachmentId);
        if (attachment == null)
            return null;

        return (attachment.FileContent, attachment.ContentType, attachment.OriginalFileName);
    }
}
