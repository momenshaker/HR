using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository abstraction for delegated authority arrangements.
/// </summary>
public interface IDelegatedAuthorityRepository
{
    Task<IReadOnlyCollection<DelegatedAuthority>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<DelegatedAuthority?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DelegatedAuthority>> GetByGrantorAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DelegatedAuthority>> GetByDelegateAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<DelegatedAuthority> AddAsync(DelegatedAuthority authority, CancellationToken cancellationToken = default);

    Task<DelegatedAuthority?> UpdateAsync(DelegatedAuthority authority, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default);
}
