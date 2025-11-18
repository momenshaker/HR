using HR.Api.Filters;
using HR.Api.Middleware;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Manages performance cycles lifecycle including activation and closure.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/performance/cycles")]
[Authorize(Roles = "Admin,HR,Manager")]
[AuditResource("PerformanceCycle")]
[FeatureRequirement(HrFeature.PerformanceManagement)]
[RequiresSubscriptionEntitlement(HrFeature.PerformanceManagement)]
public sealed class PerformanceCyclesController(IPerformanceManagementService performanceService) : ControllerBase
{
    private readonly IPerformanceManagementService _performanceService = performanceService;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<PerformanceCycleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var cycles = await _performanceService.GetCyclesAsync(cancellationToken).ConfigureAwait(false);
        return Ok(cycles);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PerformanceCycleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var cycle = await _performanceService.GetCycleAsync(id, cancellationToken).ConfigureAwait(false);
        return cycle is null ? NotFound() : Ok(cycle);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PerformanceCycleDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostAsync([FromBody] CreatePerformanceCycleRequest request, CancellationToken cancellationToken)
    {
        var created = await _performanceService.CreateCycleAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PerformanceCycleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdatePerformanceCycleRequest request, CancellationToken cancellationToken)
    {
        var updated = await _performanceService.UpdateCycleAsync(id, request, cancellationToken).ConfigureAwait(false);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(typeof(PerformanceCycleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateAsync(Guid id, CancellationToken cancellationToken)
    {
        var activated = await _performanceService.ActivateCycleAsync(id, cancellationToken).ConfigureAwait(false);
        return activated is null ? NotFound() : Ok(activated);
    }

    [HttpPost("{id:guid}/close")]
    [ProducesResponseType(typeof(PerformanceCycleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CloseAsync(Guid id, CancellationToken cancellationToken)
    {
        var closed = await _performanceService.CloseCycleAsync(id, cancellationToken).ConfigureAwait(false);
        return closed is null ? NotFound() : Ok(closed);
    }

    [HttpGet("{id:guid}/evaluations")]
    [ProducesResponseType(typeof(IReadOnlyCollection<EvaluationSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEvaluationsAsync(Guid id, CancellationToken cancellationToken)
    {
        var evaluations = await _performanceService.GetEvaluationsForCycleAsync(id, cancellationToken).ConfigureAwait(false);
        return Ok(evaluations);
    }
}
