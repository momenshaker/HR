using System;
using System.Collections.Generic;

namespace HR.Domain.Entities;

/// <summary>
///     Represents a department or organizational unit within the company hierarchy.
/// </summary>
public sealed class Department
{
    public Department(
        Guid id,
        Guid organizationId,
        string name,
        string path,
        int level,
        DateTime createdAtUtc,
        bool isActive = true,
        Guid? parentDepartmentId = null,
        string? code = null)
    {
        Id = id;
        OrganizationId = organizationId;
        Name = name;
        Path = path;
        Level = level;
        CreatedAtUtc = createdAtUtc;
        IsActive = isActive;
        ParentDepartmentId = parentDepartmentId;
        Code = code;
    }

    public Guid Id { get; }

    public Guid OrganizationId { get; }

    public Guid? ParentDepartmentId { get; }

    public string Name { get; }

    public string? Code { get; }

    public string Path { get; }

    public int Level { get; }

    public DateTime CreatedAtUtc { get; }

    public bool IsActive { get; private set; }

    public Organization? Organization { get; private set; }

    public Department? Parent { get; private set; }

    public ICollection<Department> Children { get; } = new List<Department>();

    public ICollection<EmployeeDepartment> EmployeeDepartments { get; } = new List<EmployeeDepartment>();

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

        return new Department(
            Id,
            OrganizationId,
            Name,
            path,
            level,
            CreatedAtUtc,
            IsActive,
            parentDepartmentId,
            Code);
    }
}
