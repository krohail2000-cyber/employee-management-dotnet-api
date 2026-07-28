using EmployeeManagement.Api.DTOs.Employees;

namespace EmployeeManagement.Api.Interfaces;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeResponse>> GetAllAsync(string? search, int? departmentId, bool? isActive, int page, int pageSize);
    Task<EmployeeResponse?> GetByIdAsync(int id);
    Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request);
    Task<bool> UpdateAsync(int id, UpdateEmployeeRequest request);
    Task<bool> DeleteAsync(int id);
}
