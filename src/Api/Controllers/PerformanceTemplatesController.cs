using HR.Api.Filters;
using HR.Api.Middleware;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Manages evaluation templates and rating scales used across performance cycles.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/performance/templates")]
[Authorize(Roles = "Admin,HR,Manager")]
[AuditResource("PerformanceTemplate")]
[FeatureRequirement(HrFeature.PerformanceManagement)]
[RequiresSubscriptionEntitlement(HrFeature.PerformanceManagement)]
public sealed class PerformanceTemplatesController(IPerformanceManagementService performanceService) : ControllerBase
{
    private readonly IPerformanceManagementService _performanceService = performanceService;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<EvaluationTemplateDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var templates = await _performanceService.GetTemplatesAsync(cancellationToken).ConfigureAwait(false);
        return Ok(templates);
    }

    [HttpGet("rating-scales")]
    [ProducesResponseType(typeof(IReadOnlyCollection<RatingScaleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetScalesAsync(CancellationToken cancellationToken)
    {
        var scales = await _performanceService.GetRatingScalesAsync(cancellationToken).ConfigureAwait(false);
        return Ok(scales);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EvaluationTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var template = await _performanceService.GetTemplateAsync(id, cancellationToken).ConfigureAwait(false);
        return template is null ? NotFound() : Ok(template);
    }

    [HttpPost]
    [ProducesResponseType(typeof(EvaluationTemplateDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostAsync([FromBody] CreateEvaluationTemplateRequest request, CancellationToken cancellationToken)
    {
        var created = await _performanceService.CreateTemplateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = created.Id }, created);
    }
}
