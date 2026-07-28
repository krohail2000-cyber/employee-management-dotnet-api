using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.DTOs.Employees;

public sealed record EmployeeResponse(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string JobTitle,
    DateOnly HireDate,
    decimal Salary,
    bool IsActive,
    int DepartmentId,
    string DepartmentName,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record CreateEmployeeRequest(
    [Required, StringLength(100)] string FirstName,
    [Required, StringLength(100)] string LastName,
    [Required, EmailAddress, StringLength(256)] string Email,
    [Phone, StringLength(30)] string? Phone,
    [Required, StringLength(150)] string JobTitle,
    DateOnly HireDate,
    [Range(0, double.MaxValue)] decimal Salary,
    bool IsActive,
    [Range(1, int.MaxValue)] int DepartmentId);

public sealed record UpdateEmployeeRequest(
    [Required, StringLength(100)] string FirstName,
    [Required, StringLength(100)] string LastName,
    [Required, EmailAddress, StringLength(256)] string Email,
    [Phone, StringLength(30)] string? Phone,
    [Required, StringLength(150)] string JobTitle,
    DateOnly HireDate,
    [Range(0, double.MaxValue)] decimal Salary,
    bool IsActive,
    [Range(1, int.MaxValue)] int DepartmentId);

public sealed record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
