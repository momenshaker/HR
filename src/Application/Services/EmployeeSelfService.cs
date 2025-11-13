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
    ITrainingService trainingService,
    IPositionService positionService,
    IOrganizationUnitService organizationUnitService,
    IReportingRelationshipService reportingRelationshipService,
    IDelegatedAuthorityService delegatedAuthorityService,
    ISelfServiceAccountService selfServiceAccountService) : IEmployeeSelfService
{
    private readonly IAttendanceService _attendanceService = attendanceService;
    private readonly IDelegatedAuthorityService _delegatedAuthorityService = delegatedAuthorityService;
    private readonly ILeaveManagementService _leaveService = leaveService;
    private readonly IOrganizationUnitService _organizationUnitService = organizationUnitService;
    private readonly IPayrollService _payrollService = payrollService;
    private readonly IPositionService _positionService = positionService;
    private readonly IReportingRelationshipService _reportingRelationshipService = reportingRelationshipService;
    private readonly ISelfServiceAccountService _selfServiceAccountService = selfServiceAccountService;
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
            LeaveTypeId = request.LeaveTypeId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Reason = request.Reason,
            AttachmentPath = request.AttachmentPath
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

        var timestampUtc = (request.TimestampUtc ?? DateTimeOffset.UtcNow).ToUniversalTime();
        var workDate = DateOnly.FromDateTime(timestampUtc.UtcDateTime);

        var existingRecords = await GetAttendanceHistoryAsync(employeeId, cancellationToken).ConfigureAwait(false);
        var openRecord = existingRecords.FirstOrDefault(record =>
            record.WorkDate == workDate &&
            !record.Punches.Any(punch => string.Equals(punch.Type, "ClockOut", StringComparison.OrdinalIgnoreCase)));

        if (openRecord is not null)
        {
            throw new InvalidOperationException("An open attendance record already exists for the specified work date.");
        }

        var punchRequest = new AttendancePunchRequest
        {
            Type = string.IsNullOrWhiteSpace(request.PunchType) ? "ClockIn" : request.PunchType.Trim(),
            TimestampUtc = timestampUtc,
            Notes = request.Notes?.Trim() ?? string.Empty
        };

        var createRequest = new CreateAttendanceRecordRequest
        {
            EmployeeId = employeeId,
            WorkDate = workDate,
            ShiftName = string.IsNullOrWhiteSpace(request.ShiftName) ? "Default" : request.ShiftName.Trim(),
            Punches = new[] { punchRequest },
            OvertimeMinutes = 0,
            Status = "InProgress",
            Notes = punchRequest.Notes
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

        if (attendanceRecord.Punches.Any(punch => string.Equals(punch.Type, "ClockOut", StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Attendance record is already closed.");
        }

        var timestampUtc = (request.TimestampUtc ?? DateTimeOffset.UtcNow).ToUniversalTime();

        var clockInPunch = attendanceRecord.Punches
            .Where(punch => string.Equals(punch.Type, "ClockIn", StringComparison.OrdinalIgnoreCase))
            .OrderBy(punch => punch.TimestampUtc)
            .FirstOrDefault();

        if (clockInPunch is not null && timestampUtc < clockInPunch.TimestampUtc)
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
            Punches = attendanceRecord.Punches
                .Select(punch => new AttendancePunchRequest
                {
                    Id = punch.Id,
                    Type = punch.Type,
                    TimestampUtc = punch.TimestampUtc,
                    Notes = punch.Notes
                })
                .Append(new AttendancePunchRequest
                {
                    Type = string.IsNullOrWhiteSpace(request.PunchType) ? "ClockOut" : request.PunchType.Trim(),
                    TimestampUtc = timestampUtc,
                    Notes = request.Notes?.Trim() ?? string.Empty
                })
                .ToArray(),
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
        var salarySlips = await _payrollService.GetSalarySlipsAsync(employeeId, cancellationToken).ConfigureAwait(false);
        return salarySlips;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<TrainingCourseDto>> GetTrainingCoursesAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        var trainingCourses = await _trainingService
            .GetTrainingCoursesAsync(employeeId, cancellationToken)
            .ConfigureAwait(false);

        return trainingCourses;
    }

    /// <inheritdoc />
    public async Task<EmployeeOrganizationSnapshotDto> GetOrganizationSnapshotAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        var position = await _positionService.GetByEmployeeIdAsync(employeeId, cancellationToken).ConfigureAwait(false);

        OrganizationUnitDto? organizationUnit = null;
        if (position is not null)
        {
            organizationUnit = await _organizationUnitService
                .GetByIdAsync(position.OrganizationUnitId, cancellationToken)
                .ConfigureAwait(false);
        }

        var reportingLines = position is null
            ? Array.Empty<ReportingRelationshipDto>()
            : (await Task.WhenAll(
                    _reportingRelationshipService.GetByReportPositionAsync(position.Id, cancellationToken),
                    _reportingRelationshipService.GetByManagerPositionAsync(position.Id, cancellationToken)))
                .SelectMany(result => result)
                .GroupBy(relationship => relationship.Id)
                .Select(group => group.First())
                .ToArray();

        var delegatedAuthorities = await _delegatedAuthorityService
            .GetByDelegateAsync(employeeId, cancellationToken)
            .ConfigureAwait(false);

        var account = await _selfServiceAccountService
            .GetByEmployeeIdAsync(employeeId, cancellationToken)
            .ConfigureAwait(false);

        return new EmployeeOrganizationSnapshotDto(
            employeeId,
            position,
            organizationUnit,
            reportingLines,
            delegatedAuthorities,
            account);
    }

    /// <inheritdoc />
    public Task<IReadOnlyCollection<DelegatedAuthorityDto>> GetDelegatedAuthoritiesAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return _delegatedAuthorityService.GetByDelegateAsync(employeeId, cancellationToken);
    }

    /// <inheritdoc />
    public Task<SelfServiceAccountDto?> GetAccountAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return _selfServiceAccountService.GetByEmployeeIdAsync(employeeId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SelfServiceAccountDto> RegisterAccountAsync(
        Guid employeeId,
        CreateSelfServiceAccountRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.EmployeeId != employeeId)
        {
            throw new InvalidOperationException("Self-service account request employee identifier does not match the route parameter.");
        }

        return await _selfServiceAccountService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<SelfServiceAccountDto?> UpdateAccountAsync(
        Guid employeeId,
        UpdateSelfServiceAccountRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _selfServiceAccountService.GetByEmployeeIdAsync(employeeId, cancellationToken)
            .ConfigureAwait(false);

        if (existing is null)
        {
            return null;
        }

        return await _selfServiceAccountService
            .UpdateAsync(existing.Id, request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAccountAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        var existing = await _selfServiceAccountService.GetByEmployeeIdAsync(employeeId, cancellationToken)
            .ConfigureAwait(false);

        if (existing is null)
        {
            return false;
        }

        return await _selfServiceAccountService.DeleteAsync(existing.Id, cancellationToken).ConfigureAwait(false);
    }
}
