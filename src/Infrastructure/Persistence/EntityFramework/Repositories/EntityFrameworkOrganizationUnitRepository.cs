using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkOrganizationUnitRepository : EntityFrameworkRepository<OrganizationUnit>, IOrganizationUnitRepository
{
    public EntityFrameworkOrganizationUnitRepository(HrDbContext dbContext)
        : base(dbContext, unit => unit.Id)
    {
    }

    public async Task<IReadOnlyCollection<OrganizationUnit>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllInternalAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<OrganizationUnit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(id, cancellationToken);
    }

    public Task<OrganizationUnit> AddAsync(OrganizationUnit unit, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(unit, cancellationToken);
    }

    public Task<OrganizationUnit?> UpdateAsync(OrganizationUnit unit, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(unit, cancellationToken);
    }

    public Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(id, cancellationToken);
    }
}
