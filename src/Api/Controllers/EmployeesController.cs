using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Infrastructure.Options;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Provides REST endpoints for managing employee resources.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[FeatureRequirement(HrFeature.EmployeeManagement)]
public sealed class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    /// <summary>
    ///     Retrieves all employees registered in the platform.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var employees = await _employeeService.GetAsync(cancellationToken).ConfigureAwait(false);
        return Ok(employees);
    }

    /// <summary>
    ///     Retrieves an employee by their identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var employee = await _employeeService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return employee is null ? NotFound() : Ok(employee);
    }

    /// <summary>
    ///     Creates a new employee record.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var createdEmployee = await _employeeService.CreateAsync(request, cancellationToken).ConfigureAwait(false);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = createdEmployee.Id }, createdEmployee);
    }

    /// <summary>
    ///     Updates an existing employee record.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updatedEmployee = await _employeeService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return updatedEmployee is null ? NotFound() : Ok(updatedEmployee);
    }

    /// <summary>
    ///     Deletes an employee by identifier.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _employeeService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }
}
