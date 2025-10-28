using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Request payload describing how an employee aligns to departments and cost centres.
/// </summary>
public sealed class EmployeeDepartmentAlignmentRequest
{
    [Required]
    public Guid PrimaryDepartmentId { get; init; }

    public IReadOnlyCollection<Guid> SecondaryDepartmentIds { get; init; } = Array.Empty<Guid>();

    public Guid? ReportingDepartmentId { get; init; }

    [MaxLength(50)]
    public string CostCenter { get; init; } = string.Empty;

    [MaxLength(100)]
    public string BusinessUnit { get; init; } = string.Empty;
}
