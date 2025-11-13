using System;
using System.Collections.Generic;

namespace HR.Domain.Entities;

/// <summary>
///     Represents a department or organizational unit within the company hierarchy.
/// </summary>
public sealed class Department
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public Guid? ParentDepartmentId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Code { get; init; } = string.Empty;

    public Guid? ManagerId { get; init; }

    public string Branch { get; init; } = string.Empty;

    public string Location { get; init; } = string.Empty;

    public string BusinessUnit { get; init; } = string.Empty;

    public string CostCenterCode { get; init; } = string.Empty;

    public string OperatingHours { get; init; } = string.Empty;

    public string BudgetOwner { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public bool IsActive { get; init; } = true;

    public string Path { get; init; } = string.Empty;

    public int Level { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime? UpdatedAtUtc { get; init; }

    public Organization? Organization { get; init; }

    public Department? Parent { get; init; }

    public ICollection<Department> Children { get; init; } = new List<Department>();

    public ICollection<EmployeeDepartment> EmployeeDepartments { get; init; } = new List<EmployeeDepartment>();

    /// <summary>
    ///     Creates a copy of the current department with updated hierarchy metadata.
    /// </summary>
    /// <param name="parentDepartmentId">The identifier of the new parent department or <c>null</c> for root.</param>
    /// <param name="level">The zero-based depth of the department within its organization.</param>
    /// <param name="path">The materialized path representing the department location.</param>
    /// <returns>A new <see cref="Department" /> instance containing the updated hierarchy information.</returns>
    public Department WithHierarchy(Guid? parentDepartmentId, int level, string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        return new Department
        {
            Id = Id,
            OrganizationId = OrganizationId,
            ParentDepartmentId = parentDepartmentId,
            Name = Name,
            Code = Code,
            ManagerId = ManagerId,
            Branch = Branch,
            Location = Location,
            BusinessUnit = BusinessUnit,
            CostCenterCode = CostCenterCode,
            OperatingHours = OperatingHours,
            BudgetOwner = BudgetOwner,
            Description = Description,
            IsActive = IsActive,
            Path = path,
            Level = level,
            CreatedAtUtc = CreatedAtUtc,
            UpdatedAtUtc = DateTime.UtcNow
        };
    }
}
