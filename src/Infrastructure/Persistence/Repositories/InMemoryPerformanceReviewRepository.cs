using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

/// <summary>
///     In-memory repository implementation for performance reviews.
/// </summary>
public sealed class InMemoryPerformanceReviewRepository : IPerformanceReviewRepository
{
    private readonly ConcurrentDictionary<Guid, PerformanceReview> _performanceReviews = new();

    public Task<IReadOnlyCollection<PerformanceReview>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<PerformanceReview> snapshot = _performanceReviews.Values.ToList();
        return Task.FromResult(snapshot);
    }

    public Task<PerformanceReview?> GetByIdAsync(Guid performanceReviewId, CancellationToken cancellationToken = default)
    {
        _performanceReviews.TryGetValue(performanceReviewId, out var review);
        return Task.FromResult(review);
    }

    public Task<PerformanceReview> AddAsync(PerformanceReview performanceReview, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(performanceReview);

        if (!_performanceReviews.TryAdd(performanceReview.Id, performanceReview))
        {
            throw new InvalidOperationException($"A performance review with id '{performanceReview.Id}' already exists.");
        }

        return Task.FromResult(performanceReview);
    }

    public Task<PerformanceReview?> UpdateAsync(PerformanceReview performanceReview, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(performanceReview);

        if (!_performanceReviews.ContainsKey(performanceReview.Id))
        {
            return Task.FromResult<PerformanceReview?>(null);
        }

        _performanceReviews[performanceReview.Id] = performanceReview;
        return Task.FromResult<PerformanceReview?>(performanceReview);
    }

    public Task<bool> RemoveAsync(Guid performanceReviewId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_performanceReviews.TryRemove(performanceReviewId, out _));
    }
}
