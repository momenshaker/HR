using HR.Api.Authorization;
using HR.Api.Contracts;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Provides REST endpoints for managing departments scoped to an organization.
/// </summary>

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
[RolePermission(
    "departments",
    readRoles: new[] { "Admin", "HR", "Manager" },
    writeRoles: new[] { "Admin", "Manager" })]
public sealed class DepartmentsController(IDepartmentService departmentService) : ControllerBase
{
    private readonly IDepartmentService _departmentService = departmentService;

    /// <summary>
    ///     Retrieves the departments for the specified organization either as a flat collection or a hierarchy.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<DepartmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(
        Guid organizationId,
        [FromQuery] bool hierarchy = false,
        CancellationToken cancellationToken = default)
    {
        if (hierarchy)
        {
            var tree = await _departmentService
                .GetHierarchyAsync(organizationId, cancellationToken)
                .ConfigureAwait(false);

            return Ok(tree);
        }

        var departments = await _departmentService
            .GetByOrganizationAsync(organizationId, cancellationToken)
            .ConfigureAwait(false);

        return Ok(departments);
    }

    /// <summary>
    ///     Creates a department for the specified organization.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> PostAsync(
        Guid organizationId,
        [FromBody] CreateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var created = await _departmentService
            .CreateAsync(organizationId, request, cancellationToken)
            .ConfigureAwait(false);

        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { organizationId, departmentId = created.Id },
            created);
    }

    /// <summary>
    ///     Retrieves a single department within the specified organization.
    /// </summary>
    [HttpGet("{departmentId:guid}")]
    [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(
        Guid organizationId,
        Guid departmentId,
        CancellationToken cancellationToken = default)
    {
        var department = await _departmentService
            .GetByIdAsync(organizationId, departmentId, cancellationToken)
            .ConfigureAwait(false);

        return department is null ? NotFound() : Ok(department);
    }

    /// <summary>
    ///     Updates a department scoped to the specified organization.
    /// </summary>
    [HttpPut("{departmentId:guid}")]
    [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> PutAsync(
        Guid organizationId,
        Guid departmentId,
        [FromBody] UpdateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var updated = await _departmentService
            .UpdateAsync(organizationId, departmentId, request, cancellationToken)
            .ConfigureAwait(false);

        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    ///     Moves a department under a new parent within the same organization.
    /// </summary>
    [HttpPost("{departmentId:guid}:move")]
    [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> MoveAsync(
        Guid organizationId,
        Guid departmentId,
        [FromBody] MoveDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var moved = await _departmentService
            .MoveAsync(organizationId, departmentId, request.NewParentDepartmentId, cancellationToken)
            .ConfigureAwait(false);

        return moved is null ? NotFound() : Ok(moved);
    }

    /// <summary>
    ///     Deletes a department if it has no children unless cascade deletion is requested.
    /// </summary>
    [HttpDelete("{departmentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAsync(
        Guid organizationId,
        Guid departmentId,
        [FromQuery] bool cascade = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _departmentService
            .DeleteAsync(organizationId, departmentId, cascade, cancellationToken)
            .ConfigureAwait(false);

        if (result.NotFound)
        {
            return NotFound();
        }

        if (result.BlockedByChildren)
        {
            return Conflict(new { message = "Department has child departments. Use cascade=true to delete the subtree." });
        }

        return NoContent();
    }
}

