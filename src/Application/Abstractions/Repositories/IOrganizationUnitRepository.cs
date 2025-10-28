using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository abstraction for organisation units.
/// </summary>
public interface IOrganizationUnitRepository
{
    Task<IReadOnlyCollection<OrganizationUnit>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<OrganizationUnit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<OrganizationUnit> AddAsync(OrganizationUnit unit, CancellationToken cancellationToken = default);

    Task<OrganizationUnit?> UpdateAsync(OrganizationUnit unit, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default);
}
