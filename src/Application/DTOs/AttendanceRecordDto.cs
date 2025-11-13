namespace HR.Application.DTOs;

/// <summary>
///     Read model representing an attendance record.
/// </summary>
public sealed record AttendanceRecordDto(
    Guid Id,
    Guid EmployeeId,
    DateOnly WorkDate,
    string ShiftName,
    int OvertimeMinutes,
    string Status,
    string Notes,
    IReadOnlyCollection<AttendancePunchDto> Punches);
