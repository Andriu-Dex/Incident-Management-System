using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Features.Users.Commands;

/// <summary>
/// Handler for deleting a user
/// </summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        return await _userRepository.DeleteAsync(request.UserId);
    }
}
