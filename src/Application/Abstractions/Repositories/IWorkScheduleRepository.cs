using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository abstraction for <see cref="WorkSchedule" /> aggregates.
/// </summary>
public interface IWorkScheduleRepository
{
    Task<IReadOnlyCollection<WorkSchedule>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<WorkSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<WorkSchedule> AddAsync(WorkSchedule schedule, CancellationToken cancellationToken = default);

    Task<WorkSchedule?> UpdateAsync(WorkSchedule schedule, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default);
}
