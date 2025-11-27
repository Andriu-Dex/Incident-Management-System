using System.ComponentModel.DataAnnotations;
using IncidentsTI.Domain.Enums;

namespace IncidentsTI.Application.DTOs.Knowledge;

/// <summary>
/// DTO para crear un nuevo artículo
/// </summary>
public class CreateArticleDto
{
    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Debe seleccionar un servicio")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un servicio válido")]
    public int ServiceId { get; set; }
    
    [Required(ErrorMessage = "Debe seleccionar un tipo de incidente")]
    public IncidentType IncidentType { get; set; }
    
    [Required(ErrorMessage = "La descripción del problema es obligatoria")]
    [StringLength(2000, ErrorMessage = "La descripción no puede exceder 2000 caracteres")]
    public string ProblemDescription { get; set; } = string.Empty;
    
    [StringLength(2000, ErrorMessage = "Las recomendaciones no pueden exceder 2000 caracteres")]
    public string? Recommendations { get; set; }
    
    [Range(1, 480, ErrorMessage = "El tiempo estimado debe estar entre 1 y 480 minutos (8 horas)")]
    public int? EstimatedResolutionTimeMinutes { get; set; }
    
    /// <summary>
    /// Incidente que originó esta solución (opcional)
    /// </summary>
    public int? OriginIncidentId { get; set; }
    
    /// <summary>
    /// Pasos de la solución
    /// </summary>
    public List<CreateSolutionStepDto> Steps { get; set; } = new();
    
    /// <summary>
    /// Palabras clave separadas por coma
    /// </summary>
    [StringLength(500, ErrorMessage = "Las palabras clave no pueden exceder 500 caracteres")]
    public string Keywords { get; set; } = string.Empty;
}

/// <summary>
/// DTO para actualizar un artículo existente
/// </summary>
public class UpdateArticleDto
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Debe seleccionar un servicio")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un servicio válido")]
    public int ServiceId { get; set; }
    
    [Required(ErrorMessage = "Debe seleccionar un tipo de incidente")]
    public IncidentType IncidentType { get; set; }
    
    [Required(ErrorMessage = "La descripción del problema es obligatoria")]
    [StringLength(2000, ErrorMessage = "La descripción no puede exceder 2000 caracteres")]
    public string ProblemDescription { get; set; } = string.Empty;
    
    [StringLength(2000, ErrorMessage = "Las recomendaciones no pueden exceder 2000 caracteres")]
    public string? Recommendations { get; set; }
    
    [Range(1, 480, ErrorMessage = "El tiempo estimado debe estar entre 1 y 480 minutos (8 horas)")]
    public int? EstimatedResolutionTimeMinutes { get; set; }
    
    /// <summary>
    /// Pasos de la solución
    /// </summary>
    public List<CreateSolutionStepDto> Steps { get; set; } = new();
    
    /// <summary>
    /// Palabras clave separadas por coma
    /// </summary>
    [StringLength(500, ErrorMessage = "Las palabras clave no pueden exceder 500 caracteres")]
    public string Keywords { get; set; } = string.Empty;
}
