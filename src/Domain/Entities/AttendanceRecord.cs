namespace HR.Domain.Entities;

/// <summary>
///     Represents an employee's attendance for a specific work day or shift.
/// </summary>
public sealed class AttendanceRecord
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public DateOnly WorkDate { get; set; }

    public string ShiftName { get; set; } = string.Empty;

    public DateTimeOffset? ScheduledStartTimeUtc { get; set; }

    public DateTimeOffset? ScheduledEndTimeUtc { get; set; }

    public int ScheduledWorkMinutes { get; set; }

    public int BreakMinutes { get; set; }

    public int GracePeriodMinutes { get; set; }

    public DateTimeOffset? CheckInTimeUtc { get; set; }

    public DateTimeOffset? CheckOutTimeUtc { get; set; }

    public int TotalWorkedMinutes { get; set; }

    public int LateMinutes { get; set; }

    public int EarlyLeaveMinutes { get; set; }

    public int OvertimeMinutes { get; set; }

    public int AbsenceMinutes { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    public string Remarks { get; set; } = string.Empty;

    public ICollection<AttendancePunch> Punches { get; } = new List<AttendancePunch>();
}
