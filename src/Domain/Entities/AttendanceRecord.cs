namespace HR.Domain.Entities;

/// <summary>
///     Represents an employee's attendance for a specific work day or shift.
/// </summary>
public sealed class AttendanceRecord
{
    public Guid Id { get; init; }

    public Guid EmployeeId { get; init; }

    public DateOnly WorkDate { get; init; }

    public string ShiftName { get; init; } = string.Empty;

    public int OvertimeMinutes { get; init; }

    public string Status { get; init; } = string.Empty;

    public string Notes { get; init; } = string.Empty;

    public ICollection<AttendancePunch> Punches { get; } = new List<AttendancePunch>();
}
