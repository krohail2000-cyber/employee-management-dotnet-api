using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs.Departments;
using EmployeeManagement.Api.Interfaces;
using EmployeeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Services;

public sealed class DepartmentService(AppDbContext db) : IDepartmentService
{
    public async Task<IReadOnlyCollection<DepartmentResponse>> GetAllAsync() =>
        await db.Departments.AsNoTracking().OrderBy(x => x.Name)
            .Select(x => new DepartmentResponse(x.Id, x.Name, x.Description, x.CreatedAt))
            .ToListAsync();

    public Task<DepartmentResponse?> GetByIdAsync(int id) =>
        db.Departments.AsNoTracking().Where(x => x.Id == id)
            .Select(x => new DepartmentResponse(x.Id, x.Name, x.Description, x.CreatedAt))
            .SingleOrDefaultAsync();

    public async Task<DepartmentResponse> CreateAsync(CreateDepartmentRequest request)
    {
        await EnsureUniqueNameAsync(request.Name);
        var department = new Department { Name = request.Name.Trim(), Description = request.Description?.Trim() };
        db.Departments.Add(department);
        await db.SaveChangesAsync();
        return Map(department);
    }

    public async Task<bool> UpdateAsync(int id, UpdateDepartmentRequest request)
    {
        var department = await db.Departments.FindAsync(id);
        if (department is null) return false;
        await EnsureUniqueNameAsync(request.Name, id);
        department.Name = request.Name.Trim();
        department.Description = request.Description?.Trim();
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var department = await db.Departments.FindAsync(id);
        if (department is null) return false;
        if (await db.Employees.AnyAsync(x => x.DepartmentId == id))
            throw new InvalidOperationException("A department containing employees cannot be deleted.");
        db.Departments.Remove(department);
        await db.SaveChangesAsync();
        return true;
    }

    private async Task EnsureUniqueNameAsync(string name, int? id = null)
    {
        var normalized = name.Trim();
        if (await db.Departments.AnyAsync(x => x.Name == normalized && x.Id != id))
            throw new ArgumentException("A department with this name already exists.");
    }

    private static DepartmentResponse Map(Department x) => new(x.Id, x.Name, x.Description, x.CreatedAt);
}
