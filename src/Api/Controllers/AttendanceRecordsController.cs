using HR.Api.Authorization;
using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HR.Api.Controllers;

/// <summary>
///     Provides REST endpoints for shift orchestration, time capture, entitlement policy
///     enforcement, approval routing, and real-time balance reconciliation across
///     attendance records.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
[RolePermission(
    "attendance",
    readRoles: new[] { "Admin", "HR", "Manager", "Employee" },
    writeRoles: new[] { "Admin", "HR", "Manager", "Employee" })]
[AuditResource("AttendanceRecord")]
[FeatureRequirement(HrFeature.AttendanceAndTimeTracking)]
public sealed class AttendanceRecordsController(IAttendanceService attendanceService) : ControllerBase
{
    private readonly IAttendanceService _attendanceService = attendanceService;

    /// <summary>
    ///     Retrieves all attendance records in the platform.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<AttendanceRecordDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var records = await _attendanceService.GetAsync(cancellationToken).ConfigureAwait(false);
        return Ok(records);
    }

    /// <summary>
    ///     Retrieves an attendance record by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AttendanceRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _attendanceService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return record is null ? NotFound() : Ok(record);
    }

    /// <summary>
    ///     Creates a new attendance record.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AttendanceRecordDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] CreateAttendanceRecordRequest request, CancellationToken cancellationToken)
    {

        var createdRecord = await _attendanceService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = createdRecord.Id }, createdRecord);
    }

    /// <summary>
    ///     Updates an existing attendance record.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AttendanceRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateAttendanceRecordRequest request, CancellationToken cancellationToken)
    {

        var updatedRecord = await _attendanceService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return updatedRecord is null ? NotFound() : Ok(updatedRecord);
    }

    /// <summary>
    ///     Deletes an attendance record.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _attendanceService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }
}
