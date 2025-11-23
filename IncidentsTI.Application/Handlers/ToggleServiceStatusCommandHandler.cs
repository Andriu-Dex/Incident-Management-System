using IncidentsTI.Application.Commands;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Handlers;

public class ToggleServiceStatusCommandHandler : IRequestHandler<ToggleServiceStatusCommand, bool>
{
    private readonly IServiceRepository _serviceRepository;

    public ToggleServiceStatusCommandHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<bool> Handle(ToggleServiceStatusCommand request, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(request.ServiceId);
        if (service == null)
        {
            return false;
        }

        // Toggle the IsActive status
        service.IsActive = !service.IsActive;
        service.UpdatedAt = DateTime.UtcNow;
        
        await _serviceRepository.UpdateAsync(service);

        return true;
    }
}
