using EmployeeManagement.Api.DTOs.Auth;
using EmployeeManagement.Api.Interfaces;
using EmployeeManagement.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Api.Services;

public sealed class AuthService(
    UserManager<ApplicationUser> userManager,
    TokenService tokenService) : IAuthService
{
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return null;
        }

        var roles = (await userManager.GetRolesAsync(user)).ToArray();
        var (token, expiresAt) = tokenService.CreateToken(user, roles);
        return new LoginResponse(token, expiresAt, user.Email!, roles);
    }
}
