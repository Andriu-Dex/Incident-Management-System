using IncidentsTI.Application.DTOs;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Commands;

public class UploadAttachmentCommand : IRequest<AttachmentDto?>
{
    public int IncidentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
    public string UploadedById { get; set; } = string.Empty;
}

public class UploadAttachmentCommandHandler : IRequestHandler<UploadAttachmentCommand, AttachmentDto?>
{
    private readonly IIncidentAttachmentRepository _attachmentRepository;
    private readonly IIncidentRepository _incidentRepository;

    // Configuración de límites
    private const int MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB
    private const int MaxFilesPerIncident = 5;
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/gif",
        "image/webp",
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "text/plain"
    };

    public UploadAttachmentCommandHandler(
        IIncidentAttachmentRepository attachmentRepository,
        IIncidentRepository incidentRepository)
    {
        _attachmentRepository = attachmentRepository;
        _incidentRepository = incidentRepository;
    }

    public async Task<AttachmentDto?> Handle(UploadAttachmentCommand request, CancellationToken cancellationToken)
    {
        // Validar que el incidente existe
        var incident = await _incidentRepository.GetByIdAsync(request.IncidentId);
        if (incident == null)
            return null;

        // Validar tipo de archivo
        if (!AllowedContentTypes.Contains(request.ContentType))
            throw new InvalidOperationException($"Tipo de archivo no permitido: {request.ContentType}");

        // Validar tamaño del archivo
        if (request.FileContent.Length > MaxFileSizeBytes)
            throw new InvalidOperationException($"El archivo excede el tamaño máximo permitido de {MaxFileSizeBytes / (1024 * 1024)} MB");

        // Validar cantidad de archivos por incidente
        var currentCount = await _attachmentRepository.GetAttachmentCountByIncidentAsync(request.IncidentId);
        if (currentCount >= MaxFilesPerIncident)
            throw new InvalidOperationException($"El incidente ya tiene el máximo de {MaxFilesPerIncident} archivos adjuntos");

        // Generar nombre único para el archivo
        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(request.FileName)}";

        var attachment = new IncidentAttachment
        {
            IncidentId = request.IncidentId,
            FileName = uniqueFileName,
            OriginalFileName = request.FileName,
            ContentType = request.ContentType,
            FileSize = request.FileContent.Length,
            FileContent = request.FileContent,
            UploadedById = request.UploadedById,
            UploadedAt = DateTime.UtcNow
        };

        var savedAttachment = await _attachmentRepository.AddAsync(attachment);

        return new AttachmentDto
        {
            Id = savedAttachment.Id,
            IncidentId = savedAttachment.IncidentId,
            FileName = savedAttachment.FileName,
            OriginalFileName = savedAttachment.OriginalFileName,
            ContentType = savedAttachment.ContentType,
            FileSize = savedAttachment.FileSize,
            FormattedSize = savedAttachment.FormattedSize,
            IsImage = savedAttachment.IsImage,
            UploadedById = savedAttachment.UploadedById,
            UploadedAt = savedAttachment.UploadedAt
        };
    }
}
