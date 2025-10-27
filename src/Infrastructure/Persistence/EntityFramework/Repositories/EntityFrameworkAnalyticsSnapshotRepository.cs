using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkAnalyticsSnapshotRepository : EntityFrameworkRepository<AnalyticsSnapshot>, IAnalyticsSnapshotRepository
{
    public EntityFrameworkAnalyticsSnapshotRepository(HrDbContext dbContext)
        : base(dbContext, snapshot => snapshot.Id)
    {
    }

    public async Task<IReadOnlyCollection<AnalyticsSnapshot>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllInternalAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<AnalyticsSnapshot?> GetByIdAsync(Guid analyticsSnapshotId, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(analyticsSnapshotId, cancellationToken);
    }

    public Task<AnalyticsSnapshot> AddAsync(AnalyticsSnapshot analyticsSnapshot, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(analyticsSnapshot, cancellationToken);
    }

    public Task<AnalyticsSnapshot?> UpdateAsync(AnalyticsSnapshot analyticsSnapshot, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(analyticsSnapshot, cancellationToken);
    }

    public Task<bool> RemoveAsync(Guid analyticsSnapshotId, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(analyticsSnapshotId, cancellationToken);
    }
}
