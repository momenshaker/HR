using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service exposing reporting hierarchy management.
/// </summary>
public interface IReportingRelationshipService
{
    Task<IReadOnlyCollection<ReportingRelationshipDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<ReportingRelationshipDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ReportingRelationshipDto>> GetByManagerPositionAsync(Guid managerPositionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ReportingRelationshipDto>> GetByReportPositionAsync(Guid reportPositionId, CancellationToken cancellationToken = default);

    Task<ReportingRelationshipDto> CreateAsync(CreateReportingRelationshipRequest request, CancellationToken cancellationToken = default);

    Task<ReportingRelationshipDto?> UpdateAsync(Guid id, UpdateReportingRelationshipRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
