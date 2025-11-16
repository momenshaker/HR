using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

internal sealed class InMemoryWorkScheduleRepository : IWorkScheduleRepository
{
    private readonly ConcurrentDictionary<Guid, WorkSchedule> _schedules = new();

    public Task<IReadOnlyCollection<WorkSchedule>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<WorkSchedule> snapshot = _schedules.Values.Select(Clone).ToArray();
        return Task.FromResult(snapshot);
    }

    public Task<WorkSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _schedules.TryGetValue(id, out var schedule);
        return Task.FromResult(schedule is null ? null : Clone(schedule));
    }

    public Task<WorkSchedule> AddAsync(WorkSchedule schedule, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(schedule);
        _schedules[schedule.Id] = Clone(schedule);
        return Task.FromResult(schedule);
    }

    public Task<WorkSchedule?> UpdateAsync(WorkSchedule schedule, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(schedule);

        if (!_schedules.ContainsKey(schedule.Id))
        {
            return Task.FromResult<WorkSchedule?>(null);
        }

        _schedules[schedule.Id] = Clone(schedule);
        return Task.FromResult<WorkSchedule?>(schedule);
    }

    public Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_schedules.TryRemove(id, out _));
    }

    private static WorkSchedule Clone(WorkSchedule schedule)
    {
        var clone = new WorkSchedule
        {
            Id = schedule.Id,
            Name = schedule.Name,
            OrganizationId = schedule.OrganizationId,
            DepartmentId = schedule.DepartmentId,
            IsDefaultForOrganization = schedule.IsDefaultForOrganization,
            TimeZoneId = schedule.TimeZoneId
        };

        foreach (var template in schedule.ShiftTemplates)
        {
            clone.ShiftTemplates.Add(new ShiftTemplate
            {
                Id = template.Id,
                WorkScheduleId = template.WorkScheduleId,
                DayOfWeek = template.DayOfWeek,
                StartTime = template.StartTime,
                EndTime = template.EndTime,
                BreakMinutes = template.BreakMinutes,
                GracePeriodMinutes = template.GracePeriodMinutes,
                MinimumOvertimeMinutes = template.MinimumOvertimeMinutes
            });
        }

        return clone;
    }
}
