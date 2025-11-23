using System.ComponentModel.DataAnnotations;
using IncidentsTI.Domain.Enums;

namespace IncidentsTI.Application.DTOs;

public class CreateIncidentDto
{
    [Required(ErrorMessage = "El servicio es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un servicio")]
    public int ServiceId { get; set; }
    
    [Required(ErrorMessage = "El tipo de incidente es requerido")]
    public IncidentType Type { get; set; }
    
    [Required(ErrorMessage = "La prioridad es requerida")]
    public IncidentPriority Priority { get; set; }
    
    [Required(ErrorMessage = "El título es requerido")]
    [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "La descripción es requerida")]
    [StringLength(2000, ErrorMessage = "La descripción no puede exceder 2000 caracteres")]
    public string Description { get; set; } = string.Empty;
}
