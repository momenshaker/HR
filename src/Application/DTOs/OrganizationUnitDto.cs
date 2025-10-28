namespace HR.Application.DTOs;

/// <summary>
///     Read model representing an organisation unit node.
/// </summary>
public sealed record OrganizationUnitDto(
    Guid Id,
    string Name,
    string Code,
    string Type,
    Guid? ParentUnitId,
    Guid? DepartmentId,
    Guid? LeadPositionId,
    int Level,
    string Description,
    bool IsActive);
