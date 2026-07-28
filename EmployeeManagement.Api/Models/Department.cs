using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.Models;

public sealed class Department
{
    public int Id { get; set; }

    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Employee> Employees { get; set; } = [];
}
