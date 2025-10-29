using System.Collections.Generic;
using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HR.Api.Controllers;

/// <summary>
///     Provides REST endpoints for modelling reporting hierarchies.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = "Admin,HR")]
[AuditResource("ReportingRelationship")]
[FeatureRequirement(HrFeature.OrganizationStructure)]
public sealed class ReportingRelationshipsController(IReportingRelationshipService reportingRelationshipService) : ControllerBase
{
    private readonly IReportingRelationshipService _reportingRelationshipService = reportingRelationshipService;

    /// <summary>
    ///     Retrieves all reporting relationships.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ReportingRelationshipDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var relationships = await _reportingRelationshipService.GetAsync(cancellationToken).ConfigureAwait(false);
        return Ok(relationships);
    }

    /// <summary>
    ///     Retrieves reporting lines for a manager position.
    /// </summary>
    [HttpGet("manager/{managerPositionId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ReportingRelationshipDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByManagerAsync(Guid managerPositionId, CancellationToken cancellationToken)
    {
        var relationships = await _reportingRelationshipService
            .GetByManagerPositionAsync(managerPositionId, cancellationToken)
            .ConfigureAwait(false);

        return Ok(relationships);
    }

    /// <summary>
    ///     Retrieves reporting lines for a subordinate position.
    /// </summary>
    [HttpGet("report/{reportPositionId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ReportingRelationshipDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByReportAsync(Guid reportPositionId, CancellationToken cancellationToken)
    {
        var relationships = await _reportingRelationshipService
            .GetByReportPositionAsync(reportPositionId, cancellationToken)
            .ConfigureAwait(false);

        return Ok(relationships);
    }

    /// <summary>
    ///     Retrieves a reporting relationship by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ReportingRelationshipDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var relationship = await _reportingRelationshipService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return relationship is null ? NotFound() : Ok(relationship);
    }

    /// <summary>
    ///     Creates a new reporting relationship.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ReportingRelationshipDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] CreateReportingRelationshipRequest request, CancellationToken cancellationToken)
    {

        var created = await _reportingRelationshipService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = created.Id }, created);
    }

    /// <summary>
    ///     Updates an existing reporting relationship.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ReportingRelationshipDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateReportingRelationshipRequest request, CancellationToken cancellationToken)
    {

        var updated = await _reportingRelationshipService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    ///     Deletes a reporting relationship.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _reportingRelationshipService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }
}
