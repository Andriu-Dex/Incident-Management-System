using IncidentsTI.Application.DTOs;
using MediatR;

namespace IncidentsTI.Application.Commands;

public record CreateServiceCommand(CreateServiceDto ServiceDto) : IRequest<ServiceDto>;
