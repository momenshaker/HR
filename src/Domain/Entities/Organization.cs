using System;
using System.Collections.Generic;

namespace HR.Domain.Entities;

/// <summary>
///     Represents a top-level organization that owns departments and employees.
/// </summary>
public sealed class Organization
{
    public Organization(Guid id, string name, DateTime createdAtUtc, bool isActive = true)
    {
        Id = id;
        Name = name;
        CreatedAtUtc = createdAtUtc;
        IsActive = isActive;
    }

    public Guid Id { get; }

    public string Name { get; }

    public DateTime CreatedAtUtc { get; }

    public bool IsActive { get; private set; }

    public ICollection<Department> Departments { get; } = new List<Department>();
}
