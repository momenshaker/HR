using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for employee schedule assignments.
/// </summary>
public static class EmployeeScheduleMappings
{
    public static EmployeeScheduleDto ToDto(this EmployeeSchedule schedule)
    {
        ArgumentNullException.ThrowIfNull(schedule);

        return new EmployeeScheduleDto(
            schedule.Id,
            schedule.EmployeeId,
            schedule.WorkScheduleId,
            schedule.EffectiveFrom,
            schedule.EffectiveTo);
    }

    public static EmployeeSchedule ToEntity(this CreateEmployeeScheduleRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new EmployeeSchedule
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.EmployeeId,
            WorkScheduleId = request.WorkScheduleId,
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo
        };
    }

    public static EmployeeSchedule ApplyUpdates(this UpdateEmployeeScheduleRequest request, EmployeeSchedule existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        return new EmployeeSchedule
        {
            Id = existing.Id,
            EmployeeId = request.EmployeeId,
            WorkScheduleId = request.WorkScheduleId,
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo
        };
    }
}
