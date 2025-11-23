using IncidentsTI.Application.DTOs;
using MediatR;

namespace IncidentsTI.Application.Queries;

public record GetServiceByIdQuery(int ServiceId) : IRequest<ServiceDto?>;
