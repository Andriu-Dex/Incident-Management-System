using IncidentsTI.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IncidentsTI.Application.Features.Auth.Commands;

/// <summary>
/// Handler for logout command
/// </summary>
public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LogoutCommandHandler(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _signInManager.SignOutAsync();
        return true;
    }
}
