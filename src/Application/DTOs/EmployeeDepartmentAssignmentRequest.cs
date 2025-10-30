using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming payload describing an employee's departmental assignments.
/// </summary>
public sealed class EmployeeDepartmentAssignmentRequest
{
    [Required]
    public Guid PrimaryDepartmentId { get; init; }

    public IReadOnlyCollection<Guid> SecondaryDepartmentIds { get; init; } = Array.Empty<Guid>();
}
