namespace HR.Application.DTOs;

/// <summary>
///     Read model representing an attendance record.
/// </summary>
public sealed record AttendanceRecordDto(
    Guid Id,
    Guid EmployeeId,
    DateOnly WorkDate,
    string ShiftName,
    DateTimeOffset? ScheduledStartTimeUtc,
    DateTimeOffset? ScheduledEndTimeUtc,
    DateTimeOffset? CheckInTimeUtc,
    DateTimeOffset? CheckOutTimeUtc,
    int ScheduledWorkMinutes,
    int BreakMinutes,
    int GracePeriodMinutes,
    int TotalWorkedMinutes,
    int LateMinutes,
    int EarlyLeaveMinutes,
    int OvertimeMinutes,
    int AbsenceMinutes,
    string Status,
    string Source,
    string Remarks,
    IReadOnlyCollection<AttendancePunchDto> Punches);
