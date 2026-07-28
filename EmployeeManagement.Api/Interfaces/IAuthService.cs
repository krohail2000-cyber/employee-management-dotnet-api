using EmployeeManagement.Api.DTOs.Auth;

namespace EmployeeManagement.Api.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}
