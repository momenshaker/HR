using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

internal sealed class InMemoryEmployeeScheduleRepository : IEmployeeScheduleRepository
{
    private readonly ConcurrentDictionary<Guid, EmployeeSchedule> _schedules = new();

    public Task<IReadOnlyCollection<EmployeeSchedule>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<EmployeeSchedule> snapshot = _schedules.Values.Select(Clone).ToArray();
        return Task.FromResult(snapshot);
    }

    public Task<EmployeeSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _schedules.TryGetValue(id, out var schedule);
        return Task.FromResult(schedule is null ? null : Clone(schedule));
    }

    public Task<EmployeeSchedule> AddAsync(EmployeeSchedule schedule, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(schedule);
        _schedules[schedule.Id] = Clone(schedule);
        return Task.FromResult(schedule);
    }

    public Task<EmployeeSchedule?> UpdateAsync(EmployeeSchedule schedule, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(schedule);

        if (!_schedules.ContainsKey(schedule.Id))
        {
            return Task.FromResult<EmployeeSchedule?>(null);
        }

        _schedules[schedule.Id] = Clone(schedule);
        return Task.FromResult<EmployeeSchedule?>(schedule);
    }

    public Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_schedules.TryRemove(id, out _));
    }

    private static EmployeeSchedule Clone(EmployeeSchedule schedule)
    {
        return new EmployeeSchedule
        {
            Id = schedule.Id,
            EmployeeId = schedule.EmployeeId,
            WorkScheduleId = schedule.WorkScheduleId,
            EffectiveFrom = schedule.EffectiveFrom,
            EffectiveTo = schedule.EffectiveTo
        };
    }
}
