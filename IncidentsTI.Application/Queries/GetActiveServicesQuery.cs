using IncidentsTI.Application.DTOs;
using MediatR;

namespace IncidentsTI.Application.Queries;

public record GetActiveServicesQuery : IRequest<IEnumerable<ServiceDto>>;
