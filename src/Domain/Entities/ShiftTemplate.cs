namespace HR.Domain.Entities;

/// <summary>
///     Defines the working hours and compliance rules for a specific day of the week within a work schedule.
/// </summary>
public sealed class ShiftTemplate
{
    public Guid Id { get; set; }

    public Guid WorkScheduleId { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public int BreakMinutes { get; set; }

    public int GracePeriodMinutes { get; set; }

    public int MinimumOvertimeMinutes { get; set; }

    public WorkSchedule? WorkSchedule { get; set; }
}
