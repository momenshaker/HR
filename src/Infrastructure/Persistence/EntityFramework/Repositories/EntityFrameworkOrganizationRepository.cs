using System.Linq;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkOrganizationRepository : EntityFrameworkRepository<Organization>, IOrganizationRepository
{
    public EntityFrameworkOrganizationRepository(HrDbContext dbContext)
        : base(dbContext, organization => organization.Id)
    {
    }

    public async Task<IReadOnlyCollection<Organization>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Organizations
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public Task<Organization?> GetByIdAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(organizationId, cancellationToken);
    }

    public Task<Organization> AddAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(organization, cancellationToken);
    }

    public Task<Organization?> UpdateAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(organization, cancellationToken);
    }

    public Task<bool> RemoveAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(organizationId, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        string name,
        Guid? excludingOrganizationId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var normalizedName = name.Trim().ToUpperInvariant();
        var query = DbContext.Organizations
            .AsNoTracking()
            .Where(organization => organization.Name.ToUpper() == normalizedName);

        if (excludingOrganizationId.HasValue)
        {
            query = query.Where(organization => organization.Id != excludingOrganizationId.Value);
        }

        return await query.AnyAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludingOrganizationId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        var normalizedCode = code.Trim().ToUpperInvariant();
        var query = DbContext.Organizations
            .AsNoTracking()
            .Where(organization => organization.Code == normalizedCode);

        if (excludingOrganizationId.HasValue)
        {
            query = query.Where(organization => organization.Id != excludingOrganizationId.Value);
        }

        return await query.AnyAsync(cancellationToken).ConfigureAwait(false);
    }
}
