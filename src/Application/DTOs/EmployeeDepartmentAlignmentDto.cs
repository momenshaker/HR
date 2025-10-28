namespace HR.Application.DTOs;

/// <summary>
///     Transport model capturing the departmental alignment metadata for an employee.
/// </summary>
public sealed record EmployeeDepartmentAlignmentDto(
    Guid PrimaryDepartmentId,
    IReadOnlyCollection<Guid> SecondaryDepartmentIds,
    Guid? ReportingDepartmentId,
    string CostCenter,
    string BusinessUnit);
