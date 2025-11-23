using IncidentsTI.Application.DTOs;
using MediatR;

namespace IncidentsTI.Application.Commands;

public class CreateIncidentCommand : IRequest<IncidentDto>
{
    public string UserId { get; set; } = string.Empty;
    public int ServiceId { get; set; }
    public IncidentsTI.Domain.Enums.IncidentType Type { get; set; }
    public IncidentsTI.Domain.Enums.IncidentPriority Priority { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
