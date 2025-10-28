using System.Collections.Generic;
using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Provides self-service endpoints scoped to a specific employee.
/// </summary>
[ApiController]
[Route("api/employees/{employeeId:guid}/self-service")]
[FeatureRequirement(HrFeature.EmployeeManagement)]
[FeatureRequirement(HrFeature.LeaveManagement)]
[FeatureRequirement(HrFeature.AttendanceAndTimeTracking)]
[FeatureRequirement(HrFeature.PayrollManagement)]
[FeatureRequirement(HrFeature.TrainingAndDevelopment)]
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
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

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
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

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
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

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
}
