using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository abstraction for <see cref="EmployeeSchedule" /> assignments.
/// </summary>
public interface IEmployeeScheduleRepository
{
    Task<IReadOnlyCollection<EmployeeSchedule>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<EmployeeSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<EmployeeSchedule> AddAsync(EmployeeSchedule schedule, CancellationToken cancellationToken = default);

    Task<EmployeeSchedule?> UpdateAsync(EmployeeSchedule schedule, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default);
}
