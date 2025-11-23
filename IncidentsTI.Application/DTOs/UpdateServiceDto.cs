using IncidentsTI.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace IncidentsTI.Application.DTOs;

public class UpdateServiceDto
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
    public required string Description { get; set; }

    [Required(ErrorMessage = "La categoría es obligatoria")]
    public ServiceCategory Category { get; set; }
}
