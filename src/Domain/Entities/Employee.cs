namespace HR.Domain.Entities;

/// <summary>
///     Represents an employee within the organization including master data enrichments.
/// </summary>
public sealed class Employee
{
    public Guid Id { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public DateOnly? DateOfBirth { get; init; }

    public Guid DepartmentId { get; init; }

    public DateOnly EmploymentStartDate { get; init; }

    public DateOnly? EmploymentEndDate { get; init; }

    public string JobTitle { get; init; } = string.Empty;

    /// <summary>
    ///     Captures how the employee aligns to departments, business units, and cost centres.
    /// </summary>
    public EmployeeDepartmentAlignment DepartmentAlignment { get; init; } = EmployeeDepartmentAlignment.Empty;

    /// <summary>
    ///     Captures the job architecture metadata for the employee.
    /// </summary>
    public EmployeeJobArchitecture JobArchitecture { get; init; } = EmployeeJobArchitecture.Empty;

    /// <summary>
    ///     Holds the collection of contracts that describe the employee's engagement history.
    /// </summary>
    public IReadOnlyCollection<EmploymentContract> Contracts { get; init; } = Array.Empty<EmploymentContract>();

    /// <summary>
    ///     Holds the compliance artefacts associated with the employee record.
    /// </summary>
    public IReadOnlyCollection<EmployeeComplianceDocument> ComplianceDocuments { get; init; } = Array.Empty<EmployeeComplianceDocument>();

    /// <summary>
    ///     Returns the employee's full name for display purposes.
    /// </summary>
    public string FullName => string.Join(" ", new[] { FirstName, LastName }.Where(part => !string.IsNullOrWhiteSpace(part)));
}
