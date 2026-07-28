using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.DTOs.Departments;

public sealed record DepartmentResponse(int Id, string Name, string? Description, DateTime CreatedAt);

public sealed record CreateDepartmentRequest(
    [Required, StringLength(100)] string Name,
    [StringLength(500)] string? Description);

public sealed record UpdateDepartmentRequest(
    [Required, StringLength(100)] string Name,
    [StringLength(500)] string? Description);
