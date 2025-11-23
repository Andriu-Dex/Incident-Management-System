using IncidentsTI.Application.Commands;
using IncidentsTI.Application.DTOs;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, ServiceDto>
{
    private readonly IServiceRepository _serviceRepository;

    public CreateServiceCommandHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<ServiceDto> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = new Service
        {
            Name = request.ServiceDto.Name,
            Description = request.ServiceDto.Description,
            Category = request.ServiceDto.Category,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createdService = await _serviceRepository.AddAsync(service);

        return new ServiceDto
        {
            Id = createdService.Id,
            Name = createdService.Name,
            Description = createdService.Description,
            Category = createdService.Category,
            CategoryName = createdService.Category.ToString(),
            IsActive = createdService.IsActive,
            CreatedAt = createdService.CreatedAt,
            UpdatedAt = createdService.UpdatedAt
        };
    }
}
