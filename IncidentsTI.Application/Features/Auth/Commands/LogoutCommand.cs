using MediatR;

namespace IncidentsTI.Application.Features.Auth.Commands;

/// <summary>
/// Command for user logout
/// </summary>
public class LogoutCommand : IRequest<bool>
{
}
