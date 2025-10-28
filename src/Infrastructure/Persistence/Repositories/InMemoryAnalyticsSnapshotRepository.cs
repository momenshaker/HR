using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

/// <summary>
///     In-memory repository implementation for analytics snapshots.
/// </summary>
public sealed class InMemoryAnalyticsSnapshotRepository : IAnalyticsSnapshotRepository
{
    private readonly ConcurrentDictionary<Guid, AnalyticsSnapshot> _snapshots = new();

    public Task<IReadOnlyCollection<AnalyticsSnapshot>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyCollection<AnalyticsSnapshot>>([.. _snapshots.Values]);
    }

    public Task<AnalyticsSnapshot?> GetByIdAsync(Guid analyticsSnapshotId, CancellationToken cancellationToken = default)
    {
        _snapshots.TryGetValue(analyticsSnapshotId, out var analyticsSnapshot);
        return Task.FromResult(analyticsSnapshot);
    }

    public Task<AnalyticsSnapshot> AddAsync(AnalyticsSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if (!_snapshots.TryAdd(snapshot.Id, snapshot))
        {
            throw new InvalidOperationException($"An analytics snapshot with id '{snapshot.Id}' already exists.");
        }

        return Task.FromResult(snapshot);
    }

    public Task<AnalyticsSnapshot?> UpdateAsync(AnalyticsSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if (!_snapshots.ContainsKey(snapshot.Id))
        {
            return Task.FromResult<AnalyticsSnapshot?>(null);
        }

        _snapshots[snapshot.Id] = snapshot;
        return Task.FromResult<AnalyticsSnapshot?>(snapshot);
    }

    public Task<bool> RemoveAsync(Guid analyticsSnapshotId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_snapshots.TryRemove(analyticsSnapshotId, out _));
    }
}
