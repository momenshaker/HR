using System;

namespace HR.Domain.Entities;

/// <summary>
///     Represents a node within the enterprise organisation structure hierarchy.
/// </summary>
public sealed class OrganizationUnit
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Code { get; init; } = string.Empty;

    public string Type { get; init; } = string.Empty;

    public Guid? ParentUnitId { get; init; }

    public Guid? DepartmentId { get; init; }

    public Guid? LeadPositionId { get; init; }

    public int Level { get; init; }

    public string Description { get; init; } = string.Empty;

    public bool IsActive { get; init; } = true;
}
