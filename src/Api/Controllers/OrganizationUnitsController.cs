using System.Collections.Generic;
using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Provides REST endpoints for managing organisation units and hierarchy structures.
/// </summary>
[ApiController]
[Route("api/organization-units")]
[FeatureRequirement(HrFeature.OrganizationStructure)]
public sealed class OrganizationUnitsController(IOrganizationUnitService organizationUnitService) : ControllerBase
{
    private readonly IOrganizationUnitService _organizationUnitService = organizationUnitService;

    /// <summary>
    ///     Retrieves all organisation units.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<OrganizationUnitDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var units = await _organizationUnitService.GetAsync(cancellationToken).ConfigureAwait(false);
        return Ok(units);
    }

    /// <summary>
    ///     Retrieves the organisation hierarchy as a multi-level tree.
    /// </summary>
    [HttpGet("hierarchy")]
    [ProducesResponseType(typeof(IReadOnlyCollection<OrganizationHierarchyNodeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHierarchyAsync(CancellationToken cancellationToken)
    {
        var hierarchy = await _organizationUnitService.GetHierarchyAsync(cancellationToken).ConfigureAwait(false);
        return Ok(hierarchy);
    }

    /// <summary>
    ///     Retrieves an organisation unit by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrganizationUnitDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var unit = await _organizationUnitService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return unit is null ? NotFound() : Ok(unit);
    }

    /// <summary>
    ///     Creates a new organisation unit.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrganizationUnitDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] CreateOrganizationUnitRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var createdUnit = await _organizationUnitService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = createdUnit.Id }, createdUnit);
    }

    /// <summary>
    ///     Updates an existing organisation unit.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(OrganizationUnitDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateOrganizationUnitRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updated = await _organizationUnitService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    ///     Deletes an organisation unit.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _organizationUnitService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }
}
