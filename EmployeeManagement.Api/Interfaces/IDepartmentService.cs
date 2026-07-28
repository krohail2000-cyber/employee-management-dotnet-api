using EmployeeManagement.Api.DTOs.Departments;

namespace EmployeeManagement.Api.Interfaces;

public interface IDepartmentService
{
    Task<IReadOnlyCollection<DepartmentResponse>> GetAllAsync();
    Task<DepartmentResponse?> GetByIdAsync(int id);
    Task<DepartmentResponse> CreateAsync(CreateDepartmentRequest request);
    Task<bool> UpdateAsync(int id, UpdateDepartmentRequest request);
    Task<bool> DeleteAsync(int id);
}
