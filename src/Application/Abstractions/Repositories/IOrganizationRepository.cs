using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository contract for managing <see cref="Organization" /> aggregates.
/// </summary>
public interface IOrganizationRepository
{
    Task<IReadOnlyCollection<Organization>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Organization?> GetByIdAsync(Guid organizationId, CancellationToken cancellationToken = default);

    Task<Organization> AddAsync(Organization organization, CancellationToken cancellationToken = default);

    Task<Organization?> UpdateAsync(Organization organization, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid organizationId, CancellationToken cancellationToken = default);
}
