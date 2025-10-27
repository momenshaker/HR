using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for updating an attendance record.
/// </summary>
public sealed class UpdateAttendanceRecordRequest
{
    [Required]
    public Guid EmployeeId { get; init; }

    [Required]
    public DateOnly WorkDate { get; init; }

    [MaxLength(100)]
    public string ShiftName { get; init; } = string.Empty;

    public DateTime? ClockInUtc { get; init; }

    public DateTime? ClockOutUtc { get; init; }

    [Range(0, 1440)]
    public int OvertimeMinutes { get; init; }

    [MaxLength(50)]
    public string Status { get; init; } = string.Empty;

    [MaxLength(500)]
    public string Notes { get; init; } = string.Empty;
}
