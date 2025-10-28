using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

/// <summary>
///     In-memory repository implementation for interview schedules.
/// </summary>
public sealed class InMemoryInterviewScheduleRepository : IInterviewScheduleRepository
{
    private readonly ConcurrentDictionary<Guid, InterviewSchedule> _interviews = new();

    public Task<IReadOnlyCollection<InterviewSchedule>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<InterviewSchedule> snapshot = _interviews.Values
            .OrderBy(interview => interview.ScheduledAtUtc)
            .ToArray();
        return Task.FromResult(snapshot);
    }

    public Task<InterviewSchedule?> GetByIdAsync(Guid interviewId, CancellationToken cancellationToken = default)
    {
        _interviews.TryGetValue(interviewId, out var schedule);
        return Task.FromResult(schedule);
    }

    public Task<InterviewSchedule> AddAsync(InterviewSchedule schedule, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(schedule);

        if (!_interviews.TryAdd(schedule.Id, schedule))
        {
            throw new InvalidOperationException($"An interview with id '{schedule.Id}' already exists.");
        }

        return Task.FromResult(schedule);
    }

    public Task<InterviewSchedule?> UpdateAsync(InterviewSchedule schedule, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(schedule);

        if (!_interviews.ContainsKey(schedule.Id))
        {
            return Task.FromResult<InterviewSchedule?>(null);
        }

        _interviews[schedule.Id] = schedule;
        return Task.FromResult<InterviewSchedule?>(schedule);
    }
}
