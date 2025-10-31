using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkPayrollItemRepository : IPayrollItemRepository
{
    private readonly HrDbContext _dbContext;

    public EntityFrameworkPayrollItemRepository(HrDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<PayrollItem>> GetByRunAsync(Guid runId, CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.PayrollItems
            .AsNoTracking()
            .Where(i => i.RunId == runId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return items;
    }

    public async Task<PayrollItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.PayrollItems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false);

        return entity;
    }

    public async Task AddRangeAsync(IEnumerable<PayrollItem> items, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(items);

        await _dbContext.PayrollItems.AddRangeAsync(items, cancellationToken).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveByRunAsync(Guid runId, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.PayrollItems.Where(i => i.RunId == runId).ToListAsync(cancellationToken).ConfigureAwait(false);
        if (existing.Count == 0)
        {
            return;
        }

        _dbContext.PayrollItems.RemoveRange(existing);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}

