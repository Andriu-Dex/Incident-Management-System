using IncidentsTI.Domain.Enums;
using MediatR;

namespace IncidentsTI.Application.Commands;

public class UpdateIncidentPriorityCommand : IRequest<bool>
{
    public int IncidentId { get; set; }
    public IncidentPriority NewPriority { get; set; }
    
    /// <summary>
    /// Usuario que realiza el cambio (para trazabilidad)
    /// </summary>
    public string UserId { get; set; } = string.Empty;
}
