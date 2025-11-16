using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using HR.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     REST endpoints for managing organization holiday calendars.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = "Admin,HR,Manager")]
[FeatureRequirement(HrFeature.AttendanceAndTimeTracking)]
[AuditResource("Holiday")]
public sealed class HolidaysController(IHolidayService holidayService) : ControllerBase
{
    private readonly IHolidayService _holidayService = holidayService;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<HolidayDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var holidays = await _holidayService.GetAsync(cancellationToken).ConfigureAwait(false);
        return Ok(holidays);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(HolidayDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var holiday = await _holidayService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return holiday is null ? NotFound() : Ok(holiday);
    }

    [HttpPost]
    [ProducesResponseType(typeof(HolidayDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostAsync([FromBody] CreateHolidayRequest request, CancellationToken cancellationToken)
    {
        var created = await _holidayService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(HolidayDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateHolidayRequest request, CancellationToken cancellationToken)
    {
        var updated = await _holidayService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _holidayService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }
}
