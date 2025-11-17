using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/plans")]
[Authorize(Roles = "Admin,HR")]
[AuditResource("Plan")]
[FeatureRequirement(HrFeature.PlatformServices)]
public sealed class PlansController(IPlanService planService) : ControllerBase
{
    private readonly IPlanService _planService = planService;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<PlanDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken = default)
    {
        var plans = await _planService.GetPlansAsync(cancellationToken).ConfigureAwait(false);
        return Ok(plans);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PlanDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostAsync([FromBody] CreatePlanRequest request, CancellationToken cancellationToken = default)
    {
        var created = await _planService.CreatePlanAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetAsync), null, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PlanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdatePlanRequest request, CancellationToken cancellationToken = default)
    {
        var updated = await _planService.UpdatePlanAsync(id, request, cancellationToken).ConfigureAwait(false);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await _planService.DeletePlanAsync(id, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }
}
