using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class AnalyticsService : IAnalyticsService
{
    private readonly IAnalyticsSnapshotRepository _analyticsRepository;

    public AnalyticsService(IAnalyticsSnapshotRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<AnalyticsSnapshotDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var snapshots = await _analyticsRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return snapshots.Select(snapshot => snapshot.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<AnalyticsSnapshotDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var snapshot = await _analyticsRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return snapshot?.ToDto();
    }

    /// <inheritdoc />
    public async Task<AnalyticsSnapshotDto> CreateAsync(CreateAnalyticsSnapshotRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = request.ToEntity();
        var created = await _analyticsRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<AnalyticsSnapshotDto?> UpdateAsync(Guid id, UpdateAnalyticsSnapshotRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _analyticsRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updatedEntity = request.ApplyUpdates(existing);
        var persisted = await _analyticsRepository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);

        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _analyticsRepository.RemoveAsync(id, cancellationToken);
    }
}
