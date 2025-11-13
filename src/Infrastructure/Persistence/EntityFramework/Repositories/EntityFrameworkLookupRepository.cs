using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkLookupRepository : EntityFrameworkRepository<LookupValue>, ILookupRepository
{
    public EntityFrameworkLookupRepository(HrDbContext dbContext)
        : base(dbContext, value => value.Id)
    {
    }

    public async Task<IReadOnlyCollection<LookupValue>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var values = await GetAllInternalAsync(cancellationToken).ConfigureAwait(false);
        return values;
    }

    public async Task<IReadOnlyCollection<LookupValue>> GetByCategoryAsync(
        string category,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(category);

        return await DbContext.LookupValues
            .Where(value => value.Category == category)
            .AsNoTracking()
            .OrderBy(value => value.SortOrder)
            .ThenBy(value => value.DisplayName)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public Task<LookupValue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(id, cancellationToken);
    }

    public Task<LookupValue> AddAsync(LookupValue value, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(value, cancellationToken);
    }

    public Task<LookupValue?> UpdateAsync(LookupValue value, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(value, cancellationToken);
    }

    public Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(id, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string category,
        string code,
        Guid? excludingId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(category);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        var query = DbContext.LookupValues
            .AsNoTracking()
            .Where(value => value.Category == category && value.Code == code);

        if (excludingId.HasValue)
        {
            query = query.Where(value => value.Id != excludingId.Value);
        }

        return await query.AnyAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<int> GetNextSortOrderAsync(string category, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(category);

        var max = await DbContext.LookupValues
            .Where(value => value.Category == category)
            .Select(value => (int?)value.SortOrder)
            .MaxAsync(cancellationToken)
            .ConfigureAwait(false);

        return (max ?? 0) + 1;
    }
}
