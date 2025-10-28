using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository abstraction for self-service accounts.
/// </summary>
public interface ISelfServiceAccountRepository
{
    Task<IReadOnlyCollection<SelfServiceAccount>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<SelfServiceAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<SelfServiceAccount?> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<SelfServiceAccount> AddAsync(SelfServiceAccount account, CancellationToken cancellationToken = default);

    Task<SelfServiceAccount?> UpdateAsync(SelfServiceAccount account, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default);
}
