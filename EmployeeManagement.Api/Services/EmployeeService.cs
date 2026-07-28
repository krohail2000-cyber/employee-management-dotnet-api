using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs.Employees;
using EmployeeManagement.Api.Interfaces;
using EmployeeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Services;

public sealed class EmployeeService(AppDbContext db) : IEmployeeService
{
    public async Task<PagedResult<EmployeeResponse>> GetAllAsync(
        string? search, int? departmentId, bool? isActive, int page, int pageSize)
    {
        var query = db.Employees.AsNoTracking().Include(x => x.Department).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x =>
                x.FirstName.Contains(term) ||
                x.LastName.Contains(term) ||
                x.Email.Contains(term) ||
                x.JobTitle.Contains(term));
        }
        if (departmentId.HasValue)
            query = query.Where(x => x.DepartmentId == departmentId);
        if (isActive.HasValue)
            query = query.Where(x => x.IsActive == isActive);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new EmployeeResponse(
                x.Id, x.FirstName, x.LastName, x.Email, x.Phone, x.JobTitle, x.HireDate,
                x.Salary, x.IsActive, x.DepartmentId, x.Department.Name, x.CreatedAt, x.UpdatedAt))
            .ToListAsync();

        return new PagedResult<EmployeeResponse>(
            items, page, pageSize, totalCount, (int)Math.Ceiling(totalCount / (double)pageSize));
    }

    public Task<EmployeeResponse?> GetByIdAsync(int id) =>
        db.Employees.AsNoTracking()
            .Include(x => x.Department)
            .Where(x => x.Id == id)
            .Select(x => new EmployeeResponse(
                x.Id, x.FirstName, x.LastName, x.Email, x.Phone, x.JobTitle, x.HireDate,
                x.Salary, x.IsActive, x.DepartmentId, x.Department.Name, x.CreatedAt, x.UpdatedAt))
            .SingleOrDefaultAsync();

    public async Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request)
    {
        await ValidateAsync(request.Email, request.DepartmentId);
        var employee = new Employee
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            Phone = request.Phone?.Trim(),
            JobTitle = request.JobTitle.Trim(),
            HireDate = request.HireDate,
            Salary = request.Salary,
            IsActive = request.IsActive,
            DepartmentId = request.DepartmentId
        };
        db.Employees.Add(employee);
        await db.SaveChangesAsync();
        await db.Entry(employee).Reference(x => x.Department).LoadAsync();
        return Map(employee);
    }

    public async Task<bool> UpdateAsync(int id, UpdateEmployeeRequest request)
    {
        var employee = await db.Employees.FindAsync(id);
        if (employee is null) return false;

        await ValidateAsync(request.Email, request.DepartmentId, id);
        employee.FirstName = request.FirstName.Trim();
        employee.LastName = request.LastName.Trim();
        employee.Email = request.Email.Trim().ToLowerInvariant();
        employee.Phone = request.Phone?.Trim();
        employee.JobTitle = request.JobTitle.Trim();
        employee.HireDate = request.HireDate;
        employee.Salary = request.Salary;
        employee.IsActive = request.IsActive;
        employee.DepartmentId = request.DepartmentId;
        employee.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await db.Employees.FindAsync(id);
        if (employee is null) return false;
        db.Employees.Remove(employee);
        await db.SaveChangesAsync();
        return true;
    }

    private async Task ValidateAsync(string email, int departmentId, int? employeeId = null)
    {
        if (!await db.Departments.AnyAsync(x => x.Id == departmentId))
            throw new ArgumentException("Department does not exist.");
        var normalizedEmail = email.Trim().ToLowerInvariant();
        if (await db.Employees.AnyAsync(x => x.Email == normalizedEmail && x.Id != employeeId))
            throw new ArgumentException("An employee with this email already exists.");
    }

    private static EmployeeResponse Map(Employee x) => new(
        x.Id, x.FirstName, x.LastName, x.Email, x.Phone, x.JobTitle, x.HireDate,
        x.Salary, x.IsActive, x.DepartmentId, x.Department.Name, x.CreatedAt, x.UpdatedAt);
}
