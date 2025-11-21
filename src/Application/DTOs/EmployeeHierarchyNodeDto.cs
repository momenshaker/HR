using System.Collections.Generic;

namespace HR.Application.DTOs;

/// <summary>
///     Represents a node in an employee reporting hierarchy.
/// </summary>
public sealed record EmployeeHierarchyNodeDto(
    PositionDto Position,
    EmployeeDto? Employee,
    IReadOnlyCollection<EmployeeHierarchyNodeDto> DirectReports);
