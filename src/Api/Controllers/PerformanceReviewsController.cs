using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Provides REST endpoints for performance management operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class PerformanceReviewsController : ControllerBase
{
    private readonly IPerformanceManagementService _performanceService;

    public PerformanceReviewsController(IPerformanceManagementService performanceService)
    {
        _performanceService = performanceService;
    }

    /// <summary>
    ///     Retrieves all performance reviews.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<PerformanceReviewDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var reviews = await _performanceService.GetAsync(cancellationToken).ConfigureAwait(false);
        return Ok(reviews);
    }

    /// <summary>
    ///     Retrieves a performance review by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PerformanceReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var review = await _performanceService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return review is null ? NotFound() : Ok(review);
    }

    /// <summary>
    ///     Creates a new performance review.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PerformanceReviewDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] CreatePerformanceReviewRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var createdReview = await _performanceService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = createdReview.Id }, createdReview);
    }

    /// <summary>
    ///     Updates an existing performance review.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PerformanceReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdatePerformanceReviewRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updatedReview = await _performanceService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return updatedReview is null ? NotFound() : Ok(updatedReview);
    }

    /// <summary>
    ///     Deletes a performance review.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _performanceService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }
}
