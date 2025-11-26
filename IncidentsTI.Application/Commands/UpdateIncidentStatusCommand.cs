using MediatR;

namespace IncidentsTI.Application.Commands;

public class UpdateIncidentStatusCommand : IRequest<bool>
{
    public int IncidentId { get; set; }
    public IncidentsTI.Domain.Enums.IncidentStatus NewStatus { get; set; }
    
    /// <summary>
    /// Usuario que realiza el cambio (para trazabilidad)
    /// </summary>
    public string UserId { get; set; } = string.Empty;
}
