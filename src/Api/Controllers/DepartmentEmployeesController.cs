using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Provides access to employees assigned to a specific department.
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class DepartmentEmployeesController(IEmployeeService employeeService) : ControllerBase
{
    private readonly IEmployeeService _employeeService = employeeService;

    /// <summary>
    ///     Retrieves employees assigned to the specified department.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        var employees = await _employeeService
            .GetByDepartmentAsync(departmentId, cancellationToken)
            .ConfigureAwait(false);

        return Ok(employees);
    }
}

