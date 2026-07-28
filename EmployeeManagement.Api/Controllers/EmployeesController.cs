using EmployeeManagement.Api.DTOs.Employees;
using EmployeeManagement.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin,User")]
[Route("api/[controller]")]
public sealed class EmployeesController(IEmployeeService employeeService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<EmployeeResponse>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? departmentId,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1 || pageSize is < 1 or > 100)
            return BadRequest(new { message = "Page must be at least 1 and pageSize must be between 1 and 100." });
        return Ok(await employeeService.GetAllAsync(search, departmentId, isActive, page, pageSize));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeResponse>> GetById(int id)
    {
        var employee = await employeeService.GetByIdAsync(id);
        return employee is null ? NotFound() : Ok(employee);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<EmployeeResponse>> Create(CreateEmployeeRequest request)
    {
        var employee = await employeeService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateEmployeeRequest request) =>
        await employeeService.UpdateAsync(id, request) ? NoContent() : NotFound();

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await employeeService.DeleteAsync(id) ? NoContent() : NotFound();
}
