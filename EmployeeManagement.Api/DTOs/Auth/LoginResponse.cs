namespace EmployeeManagement.Api.DTOs.Auth;

public sealed record LoginResponse(
    string Token,
    DateTime ExpiresAt,
    string Email,
    IReadOnlyCollection<string> Roles);
