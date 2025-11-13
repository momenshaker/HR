using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository abstraction for dynamic lookup values.
/// </summary>
public interface ILookupRepository
{
    Task<IReadOnlyCollection<LookupValue>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<LookupValue>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);

    Task<LookupValue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<LookupValue> AddAsync(LookupValue value, CancellationToken cancellationToken = default);

    Task<LookupValue?> UpdateAsync(LookupValue value, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string category,
        string code,
        Guid? excludingId,
        CancellationToken cancellationToken = default);

    Task<int> GetNextSortOrderAsync(string category, CancellationToken cancellationToken = default);
}
