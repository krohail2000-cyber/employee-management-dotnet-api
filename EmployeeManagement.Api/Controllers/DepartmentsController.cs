using EmployeeManagement.Api.DTOs.Departments;
using EmployeeManagement.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin,User")]
[Route("api/[controller]")]
public sealed class DepartmentsController(IDepartmentService departmentService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<DepartmentResponse>>> GetAll() =>
        Ok(await departmentService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DepartmentResponse>> GetById(int id)
    {
        var department = await departmentService.GetByIdAsync(id);
        return department is null ? NotFound() : Ok(department);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<DepartmentResponse>> Create(CreateDepartmentRequest request)
    {
        var department = await departmentService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateDepartmentRequest request) =>
        await departmentService.UpdateAsync(id, request) ? NoContent() : NotFound();

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await departmentService.DeleteAsync(id) ? NoContent() : NotFound();
}
