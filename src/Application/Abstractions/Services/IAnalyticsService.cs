using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service responsible for HR analytics operations.
/// </summary>
public interface IAnalyticsService
{
    Task<IReadOnlyCollection<AnalyticsSnapshotDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<AnalyticsSnapshotDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<AnalyticsSnapshotDto> CreateAsync(CreateAnalyticsSnapshotRequest request, CancellationToken cancellationToken = default);

    Task<AnalyticsSnapshotDto?> UpdateAsync(Guid id, UpdateAnalyticsSnapshotRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
