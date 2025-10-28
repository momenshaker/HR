using System;
using System.Linq;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class EmployeeSelfService(
    ILeaveManagementService leaveService,
    IAttendanceService attendanceService,
    IPayrollService payrollService,
    ITrainingService trainingService) : IEmployeeSelfService
{
    private readonly IAttendanceService _attendanceService = attendanceService;
    private readonly ILeaveManagementService _leaveService = leaveService;
    private readonly IPayrollService _payrollService = payrollService;
    private readonly ITrainingService _trainingService = trainingService;

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<LeaveRequestDto>> GetLeaveRequestsAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        var leaveRequests = await _leaveService.GetAsync(cancellationToken).ConfigureAwait(false);
        return leaveRequests.Where(leave => leave.EmployeeId == employeeId).ToArray();
    }

    /// <inheritdoc />
    public async Task<LeaveRequestDto> SubmitLeaveRequestAsync(
        Guid employeeId,
        CreateLeaveRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.EmployeeId != Guid.Empty && request.EmployeeId != employeeId)
        {
            throw new InvalidOperationException("Leave request employee identifier does not match the route parameter.");
        }

        var payload = new CreateLeaveRequest
        {
            EmployeeId = employeeId,
            LeaveType = request.LeaveType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Reason = request.Reason
        };

        return await _leaveService.CreateAsync(payload, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<AttendanceRecordDto>> GetAttendanceHistoryAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        var records = await _attendanceService.GetAsync(cancellationToken).ConfigureAwait(false);
        return records.Where(record => record.EmployeeId == employeeId).OrderByDescending(record => record.WorkDate).ToArray();
    }

    /// <inheritdoc />
    public async Task<AttendanceRecordDto> ClockInAsync(
        Guid employeeId,
        ClockInRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var timestampUtc = (request.TimestampUtc ?? DateTime.UtcNow).ToUniversalTime();
        var workDate = DateOnly.FromDateTime(timestampUtc);

        var existingRecords = await GetAttendanceHistoryAsync(employeeId, cancellationToken).ConfigureAwait(false);
        var openRecord = existingRecords.FirstOrDefault(record =>
            record.WorkDate == workDate &&
            record.ClockOutUtc is null);

        if (openRecord is not null)
        {
            throw new InvalidOperationException("An open attendance record already exists for the specified work date.");
        }

        var createRequest = new CreateAttendanceRecordRequest
        {
            EmployeeId = employeeId,
            WorkDate = workDate,
            ShiftName = string.IsNullOrWhiteSpace(request.ShiftName) ? "Default" : request.ShiftName.Trim(),
            ClockInUtc = timestampUtc,
            ClockOutUtc = null,
            OvertimeMinutes = 0,
            Status = "InProgress",
            Notes = request.Notes?.Trim() ?? string.Empty
        };

        return await _attendanceService.CreateAsync(createRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<AttendanceRecordDto> ClockOutAsync(
        Guid employeeId,
        Guid attendanceRecordId,
        ClockOutRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var attendanceRecord = await _attendanceService.GetByIdAsync(attendanceRecordId, cancellationToken)
            .ConfigureAwait(false);

        if (attendanceRecord is null || attendanceRecord.EmployeeId != employeeId)
        {
            throw new KeyNotFoundException("Attendance record was not found for the specified employee.");
        }

        if (attendanceRecord.ClockOutUtc is not null)
        {
            throw new InvalidOperationException("Attendance record is already closed.");
        }

        var timestampUtc = (request.TimestampUtc ?? DateTime.UtcNow).ToUniversalTime();

        if (attendanceRecord.ClockInUtc is { } clockIn && timestampUtc < clockIn)
        {
            throw new InvalidOperationException("Clock-out time cannot be earlier than the recorded clock-in time.");
        }

        var combinedNotes = string.Join(
            Environment.NewLine,
            new[] { attendanceRecord.Notes, request.Notes?.Trim() }
                .Where(note => !string.IsNullOrWhiteSpace(note)));

        var updateRequest = new UpdateAttendanceRecordRequest
        {
            EmployeeId = attendanceRecord.EmployeeId,
            WorkDate = attendanceRecord.WorkDate,
            ShiftName = attendanceRecord.ShiftName,
            ClockInUtc = attendanceRecord.ClockInUtc ?? timestampUtc,
            ClockOutUtc = timestampUtc,
            OvertimeMinutes = attendanceRecord.OvertimeMinutes,
            Status = "Completed",
            Notes = combinedNotes
        };

        var updated = await _attendanceService.UpdateAsync(attendanceRecordId, updateRequest, cancellationToken)
            .ConfigureAwait(false);

        if (updated is null)
        {
            throw new InvalidOperationException("Failed to update attendance record during clock-out.");
        }

        return updated;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<SalarySlipDto>> GetSalarySlipsAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        var payrollRuns = await _payrollService.GetAsync(cancellationToken).ConfigureAwait(false);

        return payrollRuns
            .Select(run => new SalarySlipDto(
                run.Id,
                employeeId,
                run.PeriodStart,
                run.PeriodEnd,
                run.ProcessedAtUtc,
                run.Status,
                run.TotalGrossPay,
                run.TotalNetPay,
                run.Notes))
            .OrderByDescending(slip => slip.PeriodEnd)
            .ToArray();
    }

    /// <inheritdoc />
    public Task<IReadOnlyCollection<TrainingCourseDto>> GetTrainingCoursesAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        // Currently training courses are global. In future iterations this can be extended to filter by enrolments
        // specific to the provided employee identifier.
        return _trainingService.GetAsync(cancellationToken);
    }
}
