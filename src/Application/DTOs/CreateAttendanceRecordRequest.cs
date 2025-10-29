using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for creating an attendance record.
/// </summary>
public sealed class CreateAttendanceRecordRequest : IValidatableRequest
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