using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Features.Users.Commands;

/// <summary>
/// Handler for toggling user active status
/// </summary>
public class ToggleUserStatusCommandHandler : IRequestHandler<ToggleUserStatusCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public ToggleUserStatusCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken)
    {
        return await _userRepository.ToggleActiveStatusAsync(request.UserId);
    }
}
