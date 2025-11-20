using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal abstract class EntityFrameworkRepository<TEntity>
    where TEntity : class
{
    private readonly Func<TEntity, Guid> _keySelector;

    protected EntityFrameworkRepository(HrDbContext dbContext, Func<TEntity, Guid> keySelector)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
    }

    protected HrDbContext DbContext { get; }

    protected DbSet<TEntity> DbSet => DbContext.Set<TEntity>();

    protected Guid GetEntityId(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return _keySelector(entity);
    }

    protected Task<List<TEntity>> GetAllInternalAsync(CancellationToken cancellationToken)
    {
        return DbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    protected async Task<TEntity?> GetByIdInternalAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await DbSet.FindAsync(new object?[] { id }, cancellationToken).ConfigureAwait(false);
        if (entity is not null)
        {
            Detach(entity);
        }

        return entity;
    }

    protected async Task<TEntity> AddInternalAsync(TEntity entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await DbSet.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        Detach(entity);
        return entity;
    }

    protected async Task<TEntity?> UpdateInternalAsync(TEntity entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entityId = GetEntityId(entity);
        var existing = await DbSet.FindAsync(new object?[] { entityId }, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        DbContext.Entry(existing).State = EntityState.Detached;

        DbSet.Update(entity);
        try
        {
            await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            Detach(entity);

            return entity;
        }
        catch (Exception ex)
        {
            var s = ex;
        }
        return null;
    }

    protected async Task<bool> RemoveInternalAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await DbSet.FindAsync(new object?[] { id }, cancellationToken).ConfigureAwait(false);
        if (entity is null)
        {
            return false;
        }

        DbSet.Remove(entity);
        await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    protected void Detach(TEntity entity)
    {
        var entry = DbContext.Entry(entity);
        if (entry.State != EntityState.Detached)
        {
            entry.State = EntityState.Detached;
        }
    }
}
