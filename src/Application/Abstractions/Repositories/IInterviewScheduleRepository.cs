using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository contract for managing <see cref="InterviewSchedule" /> entities.
/// </summary>
public interface IInterviewScheduleRepository
{
    Task<IReadOnlyCollection<InterviewSchedule>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<InterviewSchedule?> GetByIdAsync(Guid interviewId, CancellationToken cancellationToken = default);

    Task<InterviewSchedule> AddAsync(InterviewSchedule schedule, CancellationToken cancellationToken = default);

    Task<InterviewSchedule?> UpdateAsync(InterviewSchedule schedule, CancellationToken cancellationToken = default);
}
