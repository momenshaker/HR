using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Provides access to employees assigned to a specific department.
/// </summary>
[ApiController]
[Route("api/organizations/{organizationId:guid}/departments/{departmentId:guid}/employees")]
public sealed class DepartmentEmployeesController(
    IEmployeeService employeeService,
    IDepartmentService departmentService) : ControllerBase
{
    private readonly IEmployeeService _employeeService = employeeService;
    private readonly IDepartmentService _departmentService = departmentService;

    /// <summary>
    ///     Retrieves employees assigned to the specified department.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync(
        Guid organizationId,
        Guid departmentId,
        CancellationToken cancellationToken = default)
    {
        var department = await _departmentService
            .GetByIdAsync(organizationId, departmentId, cancellationToken)
            .ConfigureAwait(false);

        if (department is null)
        {
            return NotFound();
        }

        var employees = await _employeeService
            .GetByDepartmentAsync(organizationId, departmentId, cancellationToken)
            .ConfigureAwait(false);

        return Ok(employees);
    }
}
