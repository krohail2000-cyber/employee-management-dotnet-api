using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.Api.Models;

public sealed class Employee
{
    public int Id { get; set; }

    [MaxLength(100)]
    public required string FirstName { get; set; }

    [MaxLength(100)]
    public required string LastName { get; set; }

    [MaxLength(256)]
    public required string Email { get; set; }

    [MaxLength(30)]
    public string? Phone { get; set; }

    [MaxLength(150)]
    public required string JobTitle { get; set; }

    public DateOnly HireDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Salary { get; set; }

    public bool IsActive { get; set; } = true;
    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
