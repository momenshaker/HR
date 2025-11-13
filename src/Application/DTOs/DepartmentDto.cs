using System;
using System.Collections.Generic;

namespace HR.Application.DTOs;

/// <summary>
///     Read model representing a department in the organization hierarchy.
/// </summary>
public sealed record DepartmentDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Code { get; init; } = string.Empty;

    public Guid OrganizationId { get; init; }

    public Guid? ParentDepartmentId { get; init; }

    public Guid? ManagerId { get; init; }

    public string Branch { get; init; } = string.Empty;

    public string Location { get; init; } = string.Empty;

    public string BusinessUnit { get; init; } = string.Empty;

    public string CostCenterCode { get; init; } = string.Empty;

    public string OperatingHours { get; init; } = string.Empty;

    public string BudgetOwner { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    public string Path { get; init; } = string.Empty;

    public int Level { get; init; }

    /// <summary>
    ///     Child departments when the hierarchy representation is requested.
    /// </summary>
    public IReadOnlyCollection<DepartmentDto> Children { get; init; } = Array.Empty<DepartmentDto>();
}
