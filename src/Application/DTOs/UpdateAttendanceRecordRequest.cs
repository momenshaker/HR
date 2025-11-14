using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for updating an attendance record.
/// </summary>
public sealed class UpdateAttendanceRecordRequest : IValidatableRequest
{
    [Required]
    public Guid EmployeeId { get; init; }

    [Required]
    public DateOnly WorkDate { get; init; }

    [MaxLength(100)]
    public string ShiftName { get; init; } = string.Empty;

    public DateTimeOffset? ScheduledStartTimeUtc { get; init; }

    public DateTimeOffset? ScheduledEndTimeUtc { get; init; }

    public DateTimeOffset? CheckInTimeUtc { get; init; }

    public DateTimeOffset? CheckOutTimeUtc { get; init; }

    [Range(0, 1440)]
    public int ScheduledWorkMinutes { get; init; }

    [Range(0, 180)]
    public int BreakMinutes { get; init; }

    [Range(0, 120)]
    public int GracePeriodMinutes { get; init; }

    public IReadOnlyCollection<AttendancePunchRequest> Punches { get; init; } = Array.Empty<AttendancePunchRequest>();

    [Range(0, 1440)]
    public int OvertimeMinutes { get; init; }

    [Range(0, 1440)]
    public int TotalWorkedMinutes { get; init; }

    [Range(0, 1440)]
    public int LateMinutes { get; init; }

    [Range(0, 1440)]
    public int EarlyLeaveMinutes { get; init; }

    [Range(0, 1440)]
    public int AbsenceMinutes { get; init; }

    [MaxLength(50)]
    public string Status { get; init; } = string.Empty;

    [MaxLength(50)]
    public string Source { get; init; } = string.Empty;

    [MaxLength(500)]
    public string Remarks { get; init; } = string.Empty;
}
