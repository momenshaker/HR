using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository contract for managing <see cref="PerformanceReview" /> aggregates.
/// </summary>
public interface IPerformanceReviewRepository
{
    Task<IReadOnlyCollection<PerformanceReview>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PerformanceReview?> GetByIdAsync(Guid performanceReviewId, CancellationToken cancellationToken = default);

    Task<PerformanceReview> AddAsync(PerformanceReview performanceReview, CancellationToken cancellationToken = default);

    Task<PerformanceReview?> UpdateAsync(PerformanceReview performanceReview, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid performanceReviewId, CancellationToken cancellationToken = default);
}
