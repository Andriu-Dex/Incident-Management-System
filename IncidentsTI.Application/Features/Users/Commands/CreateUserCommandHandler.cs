using IncidentsTI.Application.DTOs.Users;
using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Interfaces;
using MediatR;

namespace IncidentsTI.Application.Features.Users.Commands;

/// <summary>
/// Handler for creating a new user
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsActive = true,
            EmailConfirmed = true
        };

        var createdUser = await _userRepository.CreateAsync(user, request.Password);
        await _userRepository.AddToRoleAsync(createdUser, request.Role);

        var roles = await _userRepository.GetUserRolesAsync(createdUser);

        return new UserDto
        {
            Id = createdUser.Id,
            UserName = createdUser.UserName!,
            Email = createdUser.Email!,
            FirstName = createdUser.FirstName,
            LastName = createdUser.LastName,
            IsActive = createdUser.IsActive,
            Roles = roles.ToList(),
            CreatedAt = createdUser.CreatedAt
        };
    }
}
