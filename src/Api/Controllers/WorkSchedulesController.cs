using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using HR.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     REST endpoints for configuring work schedules and shifts.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = "Admin,HR,Manager")]
[FeatureRequirement(HrFeature.AttendanceAndTimeTracking)]
[AuditResource("WorkSchedule")]
public sealed class WorkSchedulesController(IWorkScheduleService workScheduleService) : ControllerBase
{
    private readonly IWorkScheduleService _workScheduleService = workScheduleService;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<WorkScheduleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var schedules = await _workScheduleService.GetAsync(cancellationToken).ConfigureAwait(false);
        return Ok(schedules);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WorkScheduleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var schedule = await _workScheduleService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return schedule is null ? NotFound() : Ok(schedule);
    }

    [HttpPost]
    [ProducesResponseType(typeof(WorkScheduleDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostAsync([FromBody] CreateWorkScheduleRequest request, CancellationToken cancellationToken)
    {
        var created = await _workScheduleService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(WorkScheduleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateWorkScheduleRequest request, CancellationToken cancellationToken)
    {
        var updated = await _workScheduleService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _workScheduleService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }
}
