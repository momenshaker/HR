using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class PerformanceManagementService : IPerformanceManagementService
{
    private readonly IPerformanceReviewRepository _reviewRepository;

    public PerformanceManagementService(IPerformanceReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<PerformanceReviewDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var reviews = await _reviewRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return reviews.Select(review => review.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<PerformanceReviewDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var review = await _reviewRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return review?.ToDto();
    }

    /// <inheritdoc />
    public async Task<PerformanceReviewDto> CreateAsync(CreatePerformanceReviewRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = request.ToEntity();
        var created = await _reviewRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<PerformanceReviewDto?> UpdateAsync(Guid id, UpdatePerformanceReviewRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _reviewRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updatedEntity = request.ApplyUpdates(existing);
        var persisted = await _reviewRepository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);

        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _reviewRepository.RemoveAsync(id, cancellationToken);
    }
}
