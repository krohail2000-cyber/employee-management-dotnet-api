using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.DTOs.Auth;

public sealed record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);
