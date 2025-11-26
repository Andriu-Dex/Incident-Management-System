using MediatR;

namespace IncidentsTI.Application.Commands;

public class UpdateIncidentServiceCommand : IRequest<bool>
{
    public int IncidentId { get; set; }
    public int NewServiceId { get; set; }
    
    /// <summary>
    /// Usuario que realiza el cambio (para trazabilidad)
    /// </summary>
    public string UserId { get; set; } = string.Empty;
}
