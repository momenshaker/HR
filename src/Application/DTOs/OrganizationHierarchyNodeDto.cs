using System.Collections.Generic;

namespace HR.Application.DTOs;

/// <summary>
///     Represents a node in a multi-level organisation hierarchy tree.
/// </summary>
public sealed record OrganizationHierarchyNodeDto(
    OrganizationUnitDto Unit,
    IReadOnlyCollection<OrganizationHierarchyNodeDto> Children);
