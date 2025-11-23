using IncidentsTI.Application.DTOs;
using MediatR;

namespace IncidentsTI.Application.Commands;

public record UpdateServiceCommand(UpdateServiceDto ServiceDto) : IRequest<ServiceDto>;
