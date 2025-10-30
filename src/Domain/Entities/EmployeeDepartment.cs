namespace HR.Domain.Entities;

/// <summary>
///     Join entity linking employees to the departments they belong to.
/// </summary>
public sealed class EmployeeDepartment
{
    public Guid EmployeeId { get; init; }

    public Guid DepartmentId { get; init; }

    public bool IsPrimary { get; init; }
}
