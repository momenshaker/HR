using HR.Api.Filters;
using HR.Api.Middleware;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Handles the lifecycle of an individual evaluation including self/manager submissions.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/performance/evaluations")]
[Authorize(Roles = "Admin,HR,Manager,Employee")]
[AuditResource("PerformanceEvaluation")]
[FeatureRequirement(HrFeature.PerformanceManagement)]
[RequiresSubscriptionEntitlement(HrFeature.PerformanceManagement)]
public sealed class PerformanceEvaluationsController(IPerformanceManagementService performanceService) : ControllerBase
{
    private readonly IPerformanceManagementService _performanceService = performanceService;

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EvaluationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var evaluation = await _performanceService.GetEvaluationAsync(id, cancellationToken).ConfigureAwait(false);
        return evaluation is null ? NotFound() : Ok(evaluation);
    }

    [HttpPut("{id:guid}/self")]
    [ProducesResponseType(typeof(EvaluationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SubmitSelfAsync(Guid id, [FromBody] SubmitEvaluationRequest request, CancellationToken cancellationToken)
    {
        var updated = await _performanceService.SubmitSelfEvaluationAsync(id, request, cancellationToken).ConfigureAwait(false);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPut("{id:guid}/manager")]
    [ProducesResponseType(typeof(EvaluationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SubmitManagerAsync(Guid id, [FromBody] SubmitEvaluationRequest request, CancellationToken cancellationToken)
    {
        var updated = await _performanceService.SubmitManagerEvaluationAsync(id, request, cancellationToken).ConfigureAwait(false);
        return updated is null ? NotFound() : Ok(updated);
    }
}
