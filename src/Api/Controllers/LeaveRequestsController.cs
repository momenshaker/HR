using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Provides REST endpoints for leave management operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[FeatureRequirement(HrFeature.LeaveManagement)]
public sealed class LeaveRequestsController(ILeaveManagementService leaveService) : ControllerBase
{
    private readonly ILeaveManagementService _leaveService = leaveService;

    /// <summary>
    ///     Retrieves all leave requests.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<LeaveRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var leaves = await _leaveService.GetAsync(cancellationToken).ConfigureAwait(false);
        return Ok(leaves);
    }

    /// <summary>
    ///     Retrieves a leave request by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LeaveRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var leave = await _leaveService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return leave is null ? NotFound() : Ok(leave);
    }

    /// <summary>
    ///     Creates a new leave request.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(LeaveRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] CreateLeaveRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var createdLeave = await _leaveService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = createdLeave.Id }, createdLeave);
    }

    /// <summary>
    ///     Updates an existing leave request.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(LeaveRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateLeaveRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updatedLeave = await _leaveService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return updatedLeave is null ? NotFound() : Ok(updatedLeave);
    }

    /// <summary>
    ///     Deletes a leave request.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _leaveService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }
}
