using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.Abstractions.Repositories;
using HR.Application.Configuration;
using HR.Application.DTOs;
using HR.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/time/timesheets")]
[Authorize(Roles = "Admin,HR,Manager")] 
[AuditResource("Timesheet")]
[FeatureRequirement(HrFeature.AttendanceAndTimeTracking)]
public sealed class TimesheetsController(ITimesheetService timesheetService, IEmployeeDepartmentRepository employeeDepartmentRepository, IDepartmentRepository departmentRepository, ITimesheetRepository timesheetRepository) : ControllerBase
{
    private readonly ITimesheetService _service = timesheetService;
    private readonly IEmployeeDepartmentRepository _employeeDepartments = employeeDepartmentRepository;
    private readonly IDepartmentRepository _departments = departmentRepository;
    private readonly ITimesheetRepository _timesheets = timesheetRepository;

    [HttpGet]
    [ProducesResponseType(typeof(TimesheetDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWeek([FromQuery] Guid employeeId, [FromQuery] DateOnly weekStart, CancellationToken cancellationToken)
    {
        var result = await _service.GetWeekAsync(employeeId, weekStart, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpPut("{id:guid}/entries")]
    [ProducesResponseType(typeof(TimesheetEntryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpsertEntry(Guid id, [FromBody] UpsertTimesheetEntryRequest request, CancellationToken cancellationToken)
    {
        var entry = await _service.UpsertEntryAsync(id, request, cancellationToken).ConfigureAwait(false);
        return Ok(entry);
    }

    [HttpPost("{id:guid}:submit")]
    [ProducesResponseType(typeof(TimesheetDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Submit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.SubmitAsync(id, cancellationToken).ConfigureAwait(false);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{id:guid}:approve")]
    [ProducesResponseType(typeof(TimesheetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveTimesheetRequest request, CancellationToken cancellationToken)
    {
        var timesheet = await _timesheets.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (timesheet is null) return NotFound();

        var deptIds = await _employeeDepartments.GetDepartmentIdsByEmployeeAsync(timesheet.EmployeeId, cancellationToken).ConfigureAwait(false);
        if (deptIds.Count > 0)
        {
            var departments = await _departments.GetByIdsAsync(deptIds, cancellationToken).ConfigureAwait(false);
            var allowed = departments.Any(d => d.ManagerId == request.ManagerId);
            if (!allowed)
            {
                return Forbid();
            }
        }

        var updated = await _service.ApproveAsync(id, request.ManagerId, request.Notes, cancellationToken).ConfigureAwait(false);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPost("{id:guid}:reject")]
    [ProducesResponseType(typeof(TimesheetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectTimesheetRequest request, CancellationToken cancellationToken)
    {
        var timesheet = await _timesheets.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (timesheet is null) return NotFound();

        var deptIds = await _employeeDepartments.GetDepartmentIdsByEmployeeAsync(timesheet.EmployeeId, cancellationToken).ConfigureAwait(false);
        if (deptIds.Count > 0)
        {
            var departments = await _departments.GetByIdsAsync(deptIds, cancellationToken).ConfigureAwait(false);
            var allowed = departments.Any(d => d.ManagerId == request.ManagerId);
            if (!allowed)
            {
                return Forbid();
            }
        }

        var updated = await _service.RejectAsync(id, request.ManagerId, request.Reason, cancellationToken).ConfigureAwait(false);
        return updated is null ? NotFound() : Ok(updated);
    }
}
