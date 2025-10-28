using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkSelfServiceAccountRepository : EntityFrameworkRepository<SelfServiceAccount>, ISelfServiceAccountRepository
{
    public EntityFrameworkSelfServiceAccountRepository(HrDbContext dbContext)
        : base(dbContext, account => account.Id)
    {
    }

    public async Task<IReadOnlyCollection<SelfServiceAccount>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllInternalAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<SelfServiceAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(id, cancellationToken);
    }

    public async Task<SelfServiceAccount?> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(account => account.EmployeeId == employeeId, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<SelfServiceAccount> AddAsync(SelfServiceAccount account, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(account);

        var exists = await DbSet.AsNoTracking()
            .AnyAsync(existing => existing.EmployeeId == account.EmployeeId, cancellationToken)
            .ConfigureAwait(false);

        if (exists)
        {
            throw new InvalidOperationException("A self-service account already exists for the specified employee.");
        }

        return await AddInternalAsync(account, cancellationToken).ConfigureAwait(false);
    }

    public Task<SelfServiceAccount?> UpdateAsync(SelfServiceAccount account, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(account, cancellationToken);
    }

    public Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(id, cancellationToken);
    }
}
