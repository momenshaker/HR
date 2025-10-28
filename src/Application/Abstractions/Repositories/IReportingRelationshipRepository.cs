using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository abstraction for reporting relationships between positions.
/// </summary>
public interface IReportingRelationshipRepository
{
    Task<IReadOnlyCollection<ReportingRelationship>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<ReportingRelationship?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ReportingRelationship>> GetByManagerPositionAsync(Guid managerPositionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ReportingRelationship>> GetByReportPositionAsync(Guid reportPositionId, CancellationToken cancellationToken = default);

    Task<ReportingRelationship> AddAsync(ReportingRelationship relationship, CancellationToken cancellationToken = default);

    Task<ReportingRelationship?> UpdateAsync(ReportingRelationship relationship, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default);
}
