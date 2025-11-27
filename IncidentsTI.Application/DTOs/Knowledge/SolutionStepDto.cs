namespace IncidentsTI.Application.DTOs.Knowledge;

/// <summary>
/// DTO para un paso de solución
/// </summary>
public class SolutionStepDto
{
    public int Id { get; set; }
    public int StepNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Note { get; set; }
}

/// <summary>
/// DTO para crear/actualizar un paso de solución
/// </summary>
public class CreateSolutionStepDto
{
    public int StepNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Note { get; set; }
}
