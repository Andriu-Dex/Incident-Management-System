using IncidentsTI.Application.DTOs;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Queries;

public class GetIncidentAttachmentsQuery : IRequest<IEnumerable<AttachmentDto>>
{
    public int IncidentId { get; set; }
}

public class GetIncidentAttachmentsQueryHandler : IRequestHandler<GetIncidentAttachmentsQuery, IEnumerable<AttachmentDto>>
{
    private readonly IIncidentAttachmentRepository _attachmentRepository;

    public GetIncidentAttachmentsQueryHandler(IIncidentAttachmentRepository attachmentRepository)
    {
        _attachmentRepository = attachmentRepository;
    }

    public async Task<IEnumerable<AttachmentDto>> Handle(GetIncidentAttachmentsQuery request, CancellationToken cancellationToken)
    {
        var attachments = await _attachmentRepository.GetByIncidentIdAsync(request.IncidentId);

        return attachments.Select(a => new AttachmentDto
        {
            Id = a.Id,
            IncidentId = a.IncidentId,
            FileName = a.FileName,
            OriginalFileName = a.OriginalFileName,
            ContentType = a.ContentType,
            FileSize = a.FileSize,
            FormattedSize = a.FormattedSize,
            IsImage = a.IsImage,
            UploadedById = a.UploadedById,
            UploadedByName = $"{a.UploadedBy?.FirstName} {a.UploadedBy?.LastName}".Trim(),
            UploadedAt = a.UploadedAt
        });
    }
}
