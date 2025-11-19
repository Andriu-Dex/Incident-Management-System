using IncidentsTI.Application.DTOs.Users;
using MediatR;

namespace IncidentsTI.Application.Features.Users.Queries;

/// <summary>
/// Query for getting all users
/// </summary>
public class GetAllUsersQuery : IRequest<List<UserDto>>
{
}
