using IncidentsTI.Application.DTOs.Knowledge;
using IncidentsTI.Domain.Enums;
using MediatR;

namespace IncidentsTI.Application.Commands;

/// <summary>
/// Comando para actualizar un artículo de conocimiento existente
/// </summary>
public class UpdateKnowledgeArticleCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ServiceId { get; set; }
    public IncidentType IncidentType { get; set; }
    public string ProblemDescription { get; set; } = string.Empty;
    public string? Recommendations { get; set; }
    public int? EstimatedResolutionTimeMinutes { get; set; }
    public List<CreateSolutionStepDto> Steps { get; set; } = new();
    public string Keywords { get; set; } = string.Empty;
}
