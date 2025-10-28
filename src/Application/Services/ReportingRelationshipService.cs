using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class ReportingRelationshipService : IReportingRelationshipService
{
    private readonly IReportingRelationshipRepository _reportingRelationshipRepository;

    public ReportingRelationshipService(IReportingRelationshipRepository reportingRelationshipRepository)
    {
        _reportingRelationshipRepository = reportingRelationshipRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ReportingRelationshipDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var relationships = await _reportingRelationshipRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return relationships.Select(relationship => relationship.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<ReportingRelationshipDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var relationship = await _reportingRelationshipRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return relationship?.ToDto();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ReportingRelationshipDto>> GetByManagerPositionAsync(
        Guid managerPositionId,
        CancellationToken cancellationToken = default)
    {
        var relationships = await _reportingRelationshipRepository
            .GetByManagerPositionAsync(managerPositionId, cancellationToken)
            .ConfigureAwait(false);

        return relationships.Select(relationship => relationship.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ReportingRelationshipDto>> GetByReportPositionAsync(
        Guid reportPositionId,
        CancellationToken cancellationToken = default)
    {
        var relationships = await _reportingRelationshipRepository
            .GetByReportPositionAsync(reportPositionId, cancellationToken)
            .ConfigureAwait(false);

        return relationships.Select(relationship => relationship.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<ReportingRelationshipDto> CreateAsync(
        CreateReportingRelationshipRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = request.ToEntity();
        var created = await _reportingRelationshipRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<ReportingRelationshipDto?> UpdateAsync(
        Guid id,
        UpdateReportingRelationshipRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _reportingRelationshipRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updatedEntity = request.ApplyUpdates(existing);
        var persisted = await _reportingRelationshipRepository
            .UpdateAsync(updatedEntity, cancellationToken)
            .ConfigureAwait(false);

        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _reportingRelationshipRepository.RemoveAsync(id, cancellationToken);
    }
}
