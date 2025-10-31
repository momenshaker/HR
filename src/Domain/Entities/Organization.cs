using System;
using System.Collections.Generic;

namespace HR.Domain.Entities;

/// <summary>
///     Represents a top-level organization that owns departments and employees.
/// </summary>
public sealed class Organization
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Code { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public bool IsActive { get; init; } = true;

    public DateTime CreatedAtUtc { get; init; }

    public DateTime? UpdatedAtUtc { get; init; }

    public ICollection<Department> Departments { get; init; } = new List<Department>();
}
