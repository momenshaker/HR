using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Provides REST endpoints for managing employee resources.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = "Admin,HR,Manager")]
[AuditResource("Employee")]
[FeatureRequirement(HrFeature.EmployeeManagement)]
public sealed class EmployeesController(IEmployeeService employeeService) : ControllerBase
{
    private readonly IEmployeeService _employeeService = employeeService;

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
    ///     Performs an advanced employee search using filters, sorting, and pagination.
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PaginatedResponse<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchAsync([FromQuery] EmployeeSearchRequest request, CancellationToken cancellationToken)
    {
        var result = await _employeeService.SearchAsync(request, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    ///     Retrieves workforce analytics snapshot metrics for leadership dashboards.
    /// </summary>
    [HttpGet("insights/workforce")]
    [ProducesResponseType(typeof(EmployeeWorkforceSnapshotDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWorkforceSnapshotAsync(CancellationToken cancellationToken)
    {
        var snapshot = await _employeeService.GetWorkforceSnapshotAsync(cancellationToken).ConfigureAwait(false);
        return Ok(snapshot);
    }

    /// <summary>
    ///     Creates a new employee record.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostAsync([FromBody] CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var createdEmployee = await _employeeService.CreateAsync(request, cancellationToken).ConfigureAwait(false);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = createdEmployee.Id }, createdEmployee);
    }

    /// <summary>
    ///     Updates an existing employee record.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
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
