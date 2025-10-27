using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Provides REST endpoints for HR analytics operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class AnalyticsSnapshotsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsSnapshotsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    /// <summary>
    ///     Retrieves all analytics snapshots.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<AnalyticsSnapshotDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var snapshots = await _analyticsService.GetAsync(cancellationToken).ConfigureAwait(false);
        return Ok(snapshots);
    }

    /// <summary>
    ///     Retrieves an analytics snapshot by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AnalyticsSnapshotDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var snapshot = await _analyticsService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return snapshot is null ? NotFound() : Ok(snapshot);
    }

    /// <summary>
    ///     Creates a new analytics snapshot.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AnalyticsSnapshotDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] CreateAnalyticsSnapshotRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var createdSnapshot = await _analyticsService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = createdSnapshot.Id }, createdSnapshot);
    }

    /// <summary>
    ///     Updates an existing analytics snapshot.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AnalyticsSnapshotDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateAnalyticsSnapshotRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updatedSnapshot = await _analyticsService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return updatedSnapshot is null ? NotFound() : Ok(updatedSnapshot);
    }

    /// <summary>
    ///     Deletes an analytics snapshot.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _analyticsService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }
}
