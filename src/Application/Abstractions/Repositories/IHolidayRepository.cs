using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository abstraction for <see cref="Holiday" /> definitions.
/// </summary>
public interface IHolidayRepository
{
    Task<IReadOnlyCollection<Holiday>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Holiday?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Holiday> AddAsync(Holiday holiday, CancellationToken cancellationToken = default);

    Task<Holiday?> UpdateAsync(Holiday holiday, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default);
}
