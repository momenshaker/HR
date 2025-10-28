namespace HR.Domain.Entities;

/// <summary>
///     Captures how an employee aligns to departments, business units, and reporting structures.
/// </summary>
public sealed class EmployeeDepartmentAlignment
{
    /// <summary>
    ///     Gets a reusable empty alignment instance.
    /// </summary>
    public static EmployeeDepartmentAlignment Empty { get; } = new()
    {
        PrimaryDepartmentId = Guid.Empty,
        SecondaryDepartmentIds = Array.Empty<Guid>(),
        ReportingDepartmentId = null,
        CostCenter = string.Empty,
        BusinessUnit = string.Empty
    };

    /// <summary>
    ///     Gets the primary department that owns the employee headcount.
    /// </summary>
    public Guid PrimaryDepartmentId { get; init; }

    /// <summary>
    ///     Gets the collection of secondary or dotted-line department assignments.
    /// </summary>
    public IReadOnlyCollection<Guid> SecondaryDepartmentIds { get; init; } = Array.Empty<Guid>();

    /// <summary>
    ///     Gets the department responsible for reporting oversight when different from the primary department.
    /// </summary>
    public Guid? ReportingDepartmentId { get; init; }

    /// <summary>
    ///     Gets the cost centre reference used for financial alignment.
    /// </summary>
    public string CostCenter { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the business unit or division label associated with the employee.
    /// </summary>
    public string BusinessUnit { get; init; } = string.Empty;
}
