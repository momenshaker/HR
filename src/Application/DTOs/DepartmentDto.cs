namespace HR.Application.DTOs;

/// <summary>
///     Read model representing a department in the organization hierarchy.
/// </summary>
public sealed record DepartmentDto(
    Guid Id,
    string Name,
    string Code,
    Guid? ParentDepartmentId,
    Guid? ManagerId,
    string Branch,
    string Location,
    string Description,
    bool IsActive);
