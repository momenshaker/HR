using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Provides REST endpoints for managing employee resources.
/// </summary>
[ApiController]
[Route("api/employees")]
public sealed class EmployeesController(
    IEmployeeService employeeService,
    IEmployeeDepartmentService employeeDepartmentService,
    IDepartmentService departmentService) : ControllerBase
{
    private readonly IEmployeeService _employeeService = employeeService;
    private readonly IEmployeeDepartmentService _employeeDepartmentService = employeeDepartmentService;
    private readonly IDepartmentService _departmentService = departmentService;

    /// <summary>
    ///     Retrieves employees using optional organization-scoped search criteria.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(
        [FromQuery] Guid? orgId = null,
        [FromQuery] string? q = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var request = new EmployeeSearchRequest
        {
            OrganizationId = orgId,
            Query = q,
            PageNumber = page,
            PageSize = pageSize
        };

        var result = await _employeeService
            .SearchAsync(request, cancellationToken)
            .ConfigureAwait(false);

        return Ok(result);
    }

    /// <summary>
    ///     Creates a new employee record.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostAsync(
        [FromBody] CreateEmployeeRequest request,
        CancellationToken cancellationToken = default)
    {
        var createdEmployee = await _employeeService
            .CreateAsync(request, cancellationToken)
            .ConfigureAwait(false);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = createdEmployee.Id }, createdEmployee);
    }

    /// <summary>
    ///     Retrieves an employee by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeService
            .GetByIdAsync(id, cancellationToken)
            .ConfigureAwait(false);

        return employee is null ? NotFound() : Ok(employee);
    }

    /// <summary>
    ///     Updates an existing employee record.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(
        Guid id,
        [FromBody] UpdateEmployeeRequest request,
        CancellationToken cancellationToken = default)
    {
        var updatedEmployee = await _employeeService
            .UpdateAsync(id, request, cancellationToken)
            .ConfigureAwait(false);

        return updatedEmployee is null ? NotFound() : Ok(updatedEmployee);
    }

    /// <summary>
    ///     Deletes an employee by identifier.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await _employeeService
            .DeleteAsync(id, cancellationToken)
            .ConfigureAwait(false);

        return deleted ? NoContent() : NotFound();
    }

    /// <summary>
    ///     Retrieves the departments assigned to the specified employee.
    /// </summary>
    [HttpGet("{employeeId:guid}/departments")]
    [ProducesResponseType(typeof(IReadOnlyCollection<DepartmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDepartmentsAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var departments = await _departmentService
            .GetByEmployeeAsync(employeeId, cancellationToken)
            .ConfigureAwait(false);

        return Ok(departments);
    }

    /// <summary>
    ///     Adds additional department assignments to the specified employee.
    /// </summary>
    [HttpPost("{employeeId:guid}/departments:assign")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AssignDepartmentsAsync(
        Guid employeeId,
        [FromBody] EmployeeDepartmentIdentifiersRequest request,
        CancellationToken cancellationToken = default)
    {
        await _employeeDepartmentService
            .AssignAsync(employeeId, request.DepartmentIds, cancellationToken)
            .ConfigureAwait(false);

        return NoContent();
    }

    /// <summary>
    ///     Replaces the department assignments for the specified employee.
    /// </summary>
    [HttpPost("{employeeId:guid}/departments:replace")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ReplaceDepartmentsAsync(
        Guid employeeId,
        [FromBody] EmployeeDepartmentIdentifiersRequest request,
        CancellationToken cancellationToken = default)
    {
        await _employeeDepartmentService
            .ReplaceAsync(employeeId, request.DepartmentIds, cancellationToken)
            .ConfigureAwait(false);

        return NoContent();
    }

    /// <summary>
    ///     Removes department assignments from the specified employee.
    /// </summary>
    [HttpPost("{employeeId:guid}/departments:unassign")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UnassignDepartmentsAsync(
        Guid employeeId,
        [FromBody] EmployeeDepartmentIdentifiersRequest request,
        CancellationToken cancellationToken = default)
    {
        await _employeeDepartmentService
            .UnassignAsync(employeeId, request.DepartmentIds, cancellationToken)
            .ConfigureAwait(false);

        return NoContent();
    }
}
