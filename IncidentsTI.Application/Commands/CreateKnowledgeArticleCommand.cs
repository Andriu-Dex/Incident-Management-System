using IncidentsTI.Application.DTOs.Knowledge;
using IncidentsTI.Domain.Enums;
using MediatR;

namespace IncidentsTI.Application.Commands;

/// <summary>
/// Comando para crear un nuevo artículo de conocimiento
/// </summary>
public class CreateKnowledgeArticleCommand : IRequest<int>
{
    public string Title { get; set; } = string.Empty;
    public int ServiceId { get; set; }
    public IncidentType IncidentType { get; set; }
    public string ProblemDescription { get; set; } = string.Empty;
    public string? Recommendations { get; set; }
    public int? EstimatedResolutionTimeMinutes { get; set; }
    public string AuthorId { get; set; } = string.Empty;
    public int? OriginIncidentId { get; set; }
    public List<CreateSolutionStepDto> Steps { get; set; } = new();
    public string Keywords { get; set; } = string.Empty;
}
