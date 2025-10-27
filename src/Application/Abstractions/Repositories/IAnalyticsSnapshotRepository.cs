using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository contract for managing <see cref="AnalyticsSnapshot" /> aggregates.
/// </summary>
public interface IAnalyticsSnapshotRepository
{
    Task<IReadOnlyCollection<AnalyticsSnapshot>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<AnalyticsSnapshot?> GetByIdAsync(Guid analyticsSnapshotId, CancellationToken cancellationToken = default);

    Task<AnalyticsSnapshot> AddAsync(AnalyticsSnapshot snapshot, CancellationToken cancellationToken = default);

    Task<AnalyticsSnapshot?> UpdateAsync(AnalyticsSnapshot snapshot, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid analyticsSnapshotId, CancellationToken cancellationToken = default);
}
