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

    public string Description { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    /// <summary>
    ///     Child departments when the hierarchy representation is requested.
    /// </summary>
    public IReadOnlyCollection<DepartmentDto> Children { get; init; } = Array.Empty<DepartmentDto>();
}
