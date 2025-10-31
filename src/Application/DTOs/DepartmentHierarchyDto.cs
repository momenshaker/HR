namespace HR.Application.DTOs;

/// <summary>
///     Represents a department node and its children within an organization hierarchy.
/// </summary>
public sealed record DepartmentHierarchyDto(
    DepartmentDto Department,
    IReadOnlyCollection<DepartmentHierarchyDto> Children);
