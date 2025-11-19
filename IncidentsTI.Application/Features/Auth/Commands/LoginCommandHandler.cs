using IncidentsTI.Application.DTOs.Auth;
using IncidentsTI.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IncidentsTI.Application.Features.Auth.Commands;

/// <summary>
/// Handler for login command
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        
        if (user == null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Correo electrónico o contraseña incorrectos"
            };
        }

        if (!user.IsActive)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Su cuenta está desactivada. Contacte al administrador"
            };
        }

        var result = await _signInManager.PasswordSignInAsync(
            user.UserName!,
            request.Password,
            request.RememberMe,
            lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Correo electrónico o contraseña incorrectos"
            };
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Inicio de sesión exitoso",
            User = new AuthResponseDto.UserInfo
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = $"{user.FirstName} {user.LastName}",
                Roles = roles.ToList()
            }
        };
    }
}
