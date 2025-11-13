using System.Collections.Generic;
using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HR.Api.Controllers;

/// <summary>
///     Provides self-service endpoints scoped to a specific employee.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = "Employee")]
[AuditResource("EmployeeSelfService")]
[FeatureRequirement(HrFeature.EmployeeManagement)]
[FeatureRequirement(HrFeature.LeaveManagement)]
[FeatureRequirement(HrFeature.AttendanceAndTimeTracking)]
[FeatureRequirement(HrFeature.PayrollManagement)]
[FeatureRequirement(HrFeature.TrainingAndDevelopment)]
[FeatureRequirement(HrFeature.OrganizationStructure)]
[FeatureRequirement(HrFeature.DelegatedAuthority)]
[FeatureRequirement(HrFeature.SelfService)]
public sealed class EmployeeSelfServiceController(IEmployeeSelfService selfService) : ControllerBase
{
    private readonly IEmployeeSelfService _selfService = selfService;

    /// <summary>
    ///     Retrieves leave requests created by the employee.
    /// </summary>
    [HttpGet("leave-requests")]
    [ProducesResponseType(typeof(IReadOnlyCollection<LeaveRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLeaveRequestsAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var leaves = await _selfService.GetLeaveRequestsAsync(employeeId, cancellationToken).ConfigureAwait(false);
        return Ok(leaves);
    }

    /// <summary>
    ///     Submits a new leave request on behalf of the employee.
    /// </summary>
    [HttpPost("leave-requests")]
    [ProducesResponseType(typeof(LeaveRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitLeaveRequestAsync(
        Guid employeeId,
        [FromBody] CreateLeaveRequest request,
        CancellationToken cancellationToken)
    {

        try
        {
            var createdLeave = await _selfService
                .SubmitLeaveRequestAsync(employeeId, request, cancellationToken)
                .ConfigureAwait(false);

            return CreatedAtAction(
                nameof(LeaveRequestsController.GetByIdAsync),
                "LeaveRequests",
                new { id = createdLeave.Id },
                createdLeave);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Unable to submit leave request",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    /// <summary>
    ///     Captures a clock-in for the employee.
    /// </summary>
    [HttpPost("attendance/clock-in")]
    [ProducesResponseType(typeof(AttendanceRecordDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ClockInAsync(
        Guid employeeId,
        [FromBody] ClockInRequest request,
        CancellationToken cancellationToken)
    {

        try
        {
            var attendanceRecord = await _selfService
                .ClockInAsync(employeeId, request, cancellationToken)
                .ConfigureAwait(false);

            return CreatedAtAction(
                nameof(AttendanceRecordsController.GetByIdAsync),
                "AttendanceRecords",
                new { id = attendanceRecord.Id },
                attendanceRecord);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Unable to clock in",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    /// <summary>
    ///     Captures a clock-out for an existing attendance record.
    /// </summary>
    [HttpPost("attendance/{attendanceRecordId:guid}/clock-out")]
    [ProducesResponseType(typeof(AttendanceRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClockOutAsync(
        Guid employeeId,
        Guid attendanceRecordId,
        [FromBody] ClockOutRequest request,
        CancellationToken cancellationToken)
    {

        try
        {
            var attendanceRecord = await _selfService
                .ClockOutAsync(employeeId, attendanceRecordId, request, cancellationToken)
                .ConfigureAwait(false);

            return Ok(attendanceRecord);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Unable to clock out",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    /// <summary>
    ///     Retrieves salary slips for the employee.
    /// </summary>
    [HttpGet("salary-slips")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SalarySlipDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSalarySlipsAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var salarySlips = await _selfService.GetSalarySlipsAsync(employeeId, cancellationToken).ConfigureAwait(false);
        return Ok(salarySlips);
    }

    /// <summary>
    ///     Retrieves training courses relevant to the employee.
    /// </summary>
    [HttpGet("training-courses")]
    [ProducesResponseType(typeof(IReadOnlyCollection<TrainingCourseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrainingCoursesAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var trainingCourses = await _selfService
            .GetTrainingCoursesAsync(employeeId, cancellationToken)
            .ConfigureAwait(false);

        return Ok(trainingCourses);
    }

    /// <summary>
    ///     Retrieves the employee's organisation snapshot including position, hierarchy, and delegations.
    /// </summary>
    [HttpGet("organization")]
    [ProducesResponseType(typeof(EmployeeOrganizationSnapshotDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganizationSnapshotAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var snapshot = await _selfService.GetOrganizationSnapshotAsync(employeeId, cancellationToken).ConfigureAwait(false);
        return Ok(snapshot);
    }

    /// <summary>
    ///     Retrieves delegated authorities assigned to the employee.
    /// </summary>
    [HttpGet("delegated-authorities")]
    [ProducesResponseType(typeof(IReadOnlyCollection<DelegatedAuthorityDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDelegatedAuthoritiesAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var authorities = await _selfService.GetDelegatedAuthoritiesAsync(employeeId, cancellationToken).ConfigureAwait(false);
        return Ok(authorities);
    }

    /// <summary>
    ///     Retrieves the self-service account associated with the employee, if available.
    /// </summary>
    [HttpGet("account")]
    [ProducesResponseType(typeof(SelfServiceAccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccountAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var account = await _selfService.GetAccountAsync(employeeId, cancellationToken).ConfigureAwait(false);
        return account is null ? NotFound() : Ok(account);
    }

    /// <summary>
    ///     Registers a new self-service account for the employee.
    /// </summary>
    [HttpPost("account")]
    [ProducesResponseType(typeof(SelfServiceAccountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAccountAsync(
        Guid employeeId,
        [FromBody] CreateSelfServiceAccountRequest request,
        CancellationToken cancellationToken)
    {

        try
        {
            var account = await _selfService
                .RegisterAccountAsync(employeeId, request, cancellationToken)
                .ConfigureAwait(false);

            return CreatedAtAction(nameof(GetAccountAsync), new { employeeId }, account);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Unable to create self-service account",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    /// <summary>
    ///     Updates the employee's self-service account.
    /// </summary>
    [HttpPut("account")]
    [ProducesResponseType(typeof(SelfServiceAccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAccountAsync(
        Guid employeeId,
        [FromBody] UpdateSelfServiceAccountRequest request,
        CancellationToken cancellationToken)
    {

        var account = await _selfService
            .UpdateAccountAsync(employeeId, request, cancellationToken)
            .ConfigureAwait(false);

        return account is null ? NotFound() : Ok(account);
    }

    /// <summary>
    ///     Deletes the employee's self-service account.
    /// </summary>
    [HttpDelete("account")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAccountAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var deleted = await _selfService.DeleteAccountAsync(employeeId, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }
}

