using IncidentsTI.Application.DTOs;
using MediatR;

namespace IncidentsTI.Application.Queries;

public record GetAllServicesQuery : IRequest<IEnumerable<ServiceDto>>;
