using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkDelegatedAuthorityRepository : EntityFrameworkRepository<DelegatedAuthority>, IDelegatedAuthorityRepository
{
    public EntityFrameworkDelegatedAuthorityRepository(HrDbContext dbContext)
        : base(dbContext, authority => authority.Id)
    {
    }

    public async Task<IReadOnlyCollection<DelegatedAuthority>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllInternalAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<DelegatedAuthority?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<DelegatedAuthority>> GetByGrantorAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Where(authority => authority.GrantorEmployeeId == employeeId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<DelegatedAuthority>> GetByDelegateAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Where(authority => authority.DelegateEmployeeId == employeeId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public Task<DelegatedAuthority> AddAsync(DelegatedAuthority authority, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(authority, cancellationToken);
    }

    public Task<DelegatedAuthority?> UpdateAsync(DelegatedAuthority authority, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(authority, cancellationToken);
    }

    public Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(id, cancellationToken);
    }
}
