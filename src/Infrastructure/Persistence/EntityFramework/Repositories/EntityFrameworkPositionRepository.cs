using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkPositionRepository : EntityFrameworkRepository<Position>, IPositionRepository
{
    public EntityFrameworkPositionRepository(HrDbContext dbContext)
        : base(dbContext, position => position.Id)
    {
    }

    public async Task<IReadOnlyCollection<Position>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllInternalAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<Position?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Position>> GetByOrganizationUnitAsync(
        Guid organizationUnitId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Where(position => position.OrganizationUnitId == organizationUnitId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public Task<Position> AddAsync(Position position, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(position, cancellationToken);
    }

    public Task<Position?> UpdateAsync(Position position, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(position, cancellationToken);
    }

    public Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(id, cancellationToken);
    }
}
