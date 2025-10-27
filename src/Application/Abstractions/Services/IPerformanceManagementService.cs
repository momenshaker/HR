using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service responsible for performance management operations.
/// </summary>
public interface IPerformanceManagementService
{
    Task<IReadOnlyCollection<PerformanceReviewDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<PerformanceReviewDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PerformanceReviewDto> CreateAsync(CreatePerformanceReviewRequest request, CancellationToken cancellationToken = default);

    Task<PerformanceReviewDto?> UpdateAsync(Guid id, UpdatePerformanceReviewRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
