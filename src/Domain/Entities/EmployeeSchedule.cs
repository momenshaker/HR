namespace HR.Domain.Entities;

/// <summary>
///     Assigns a work schedule to an employee for a given date range.
/// </summary>
public sealed class EmployeeSchedule
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public Guid WorkScheduleId { get; set; }

    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public WorkSchedule? WorkSchedule { get; set; }
}
