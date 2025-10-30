namespace HR.Domain.Entities;

/// <summary>
///     Represents a department or organizational unit within the company hierarchy.
/// </summary>
public sealed class Department
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Code { get; init; } = string.Empty;

    public Guid OrganizationId { get; init; }

    public Guid? ParentDepartmentId { get; init; }

    public Guid? ManagerId { get; init; }

    public string Branch { get; init; } = string.Empty;

    public string Location { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    /// <summary>
    ///     Indicates whether the department is currently active.
    /// </summary>
    public bool IsActive { get; init; } = true;
}
