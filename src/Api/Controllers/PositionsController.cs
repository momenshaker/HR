using System.Collections.Generic;
using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Provides REST endpoints for managing organisation positions.
/// </summary>
[ApiController]
[Route("api/positions")]
[FeatureRequirement(HrFeature.OrganizationStructure)]
public sealed class PositionsController(IPositionService positionService) : ControllerBase
{
    private readonly IPositionService _positionService = positionService;

    /// <summary>
    ///     Retrieves all positions.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<PositionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var positions = await _positionService.GetAsync(cancellationToken).ConfigureAwait(false);
        return Ok(positions);
    }

    /// <summary>
    ///     Retrieves positions scoped to a specific organisation unit.
    /// </summary>
    [HttpGet("organization-unit/{organizationUnitId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<PositionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByOrganizationUnitAsync(Guid organizationUnitId, CancellationToken cancellationToken)
    {
        var positions = await _positionService
            .GetByOrganizationUnitAsync(organizationUnitId, cancellationToken)
            .ConfigureAwait(false);

        return Ok(positions);
    }

    /// <summary>
    ///     Retrieves the position occupied by the specified employee, when available.
    /// </summary>
    [HttpGet("employee/{employeeId:guid}")]
    [ProducesResponseType(typeof(PositionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEmployeeAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var position = await _positionService.GetByEmployeeIdAsync(employeeId, cancellationToken).ConfigureAwait(false);
        return position is null ? NotFound() : Ok(position);
    }

    /// <summary>
    ///     Retrieves a position by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PositionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var position = await _positionService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return position is null ? NotFound() : Ok(position);
    }

    /// <summary>
    ///     Creates a new position.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PositionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] CreatePositionRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var created = await _positionService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = created.Id }, created);
    }

    /// <summary>
    ///     Updates an existing position.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PositionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdatePositionRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updated = await _positionService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    ///     Deletes a position.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _positionService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }
}
