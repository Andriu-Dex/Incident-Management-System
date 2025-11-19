using IncidentsTI.Application.DTOs.Users;
using MediatR;

namespace IncidentsTI.Application.Features.Users.Queries;

/// <summary>
/// Query for getting user by ID
/// </summary>
public class GetUserByIdQuery : IRequest<UserDto?>
{
    public string UserId { get; set; } = string.Empty;
}
