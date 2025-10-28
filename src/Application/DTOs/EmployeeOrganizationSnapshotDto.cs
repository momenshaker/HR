using System.Collections.Generic;

namespace HR.Application.DTOs;

/// <summary>
///     Aggregated view of an employee's position, organisational context, and delegated authorities.
/// </summary>
public sealed record EmployeeOrganizationSnapshotDto(
    Guid EmployeeId,
    PositionDto? Position,
    OrganizationUnitDto? OrganizationUnit,
    IReadOnlyCollection<ReportingRelationshipDto> ReportingLines,
    IReadOnlyCollection<DelegatedAuthorityDto> DelegatedAuthorities,
    SelfServiceAccountDto? SelfServiceAccount);
