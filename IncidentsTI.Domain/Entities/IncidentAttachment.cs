namespace IncidentsTI.Domain.Entities;

/// <summary>
/// Representa un archivo adjunto asociado a un incidente.
/// Los archivos se almacenan directamente en la base de datos como VARBINARY.
/// </summary>
public class IncidentAttachment
{
    public int Id { get; set; }
    
    // Relación con el incidente
    public int IncidentId { get; set; }
    public Incident Incident { get; set; } = null!;
    
    // Información del archivo
    public string FileName { get; set; } = string.Empty;        // Nombre guardado (puede incluir GUID)
    public string OriginalFileName { get; set; } = string.Empty; // Nombre original del archivo
    public string ContentType { get; set; } = string.Empty;      // MIME type (image/png, application/pdf, etc.)
    public long FileSize { get; set; }                           // Tamaño en bytes
    
    // Contenido del archivo (almacenado en BD)
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
    
    // Metadatos
    public string UploadedById { get; set; } = string.Empty;
    public ApplicationUser UploadedBy { get; set; } = null!;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    
    // Indica si es una imagen (para preview)
    public bool IsImage => ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
    
    // Tamaño formateado para mostrar
    public string FormattedSize
    {
        get
        {
            if (FileSize < 1024)
                return $"{FileSize} B";
            if (FileSize < 1024 * 1024)
                return $"{FileSize / 1024.0:F1} KB";
            return $"{FileSize / (1024.0 * 1024.0):F1} MB";
        }
    }
}
