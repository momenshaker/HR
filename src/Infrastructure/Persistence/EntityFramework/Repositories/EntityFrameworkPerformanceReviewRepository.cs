using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkPerformanceReviewRepository : EntityFrameworkRepository<PerformanceReview>, IPerformanceReviewRepository
{
    public EntityFrameworkPerformanceReviewRepository(HrDbContext dbContext)
        : base(dbContext, review => review.Id)
    {
    }

    public async Task<IReadOnlyCollection<PerformanceReview>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllInternalAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<PerformanceReview?> GetByIdAsync(Guid performanceReviewId, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(performanceReviewId, cancellationToken);
    }

    public Task<PerformanceReview> AddAsync(PerformanceReview performanceReview, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(performanceReview, cancellationToken);
    }

    public Task<PerformanceReview?> UpdateAsync(PerformanceReview performanceReview, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(performanceReview, cancellationToken);
    }

    public Task<bool> RemoveAsync(Guid performanceReviewId, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(performanceReviewId, cancellationToken);
    }
}
