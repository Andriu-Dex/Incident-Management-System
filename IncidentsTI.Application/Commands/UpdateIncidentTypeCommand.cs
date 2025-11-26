using IncidentsTI.Domain.Enums;
using MediatR;

namespace IncidentsTI.Application.Commands;

public class UpdateIncidentTypeCommand : IRequest<bool>
{
    public int IncidentId { get; set; }
    public IncidentType NewType { get; set; }
}
