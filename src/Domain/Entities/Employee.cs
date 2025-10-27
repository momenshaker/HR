namespace HR.Domain.Entities;

/// <summary>
///     Represents an employee within the organization.
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
    ///     Returns the employee's full name for display purposes.
    /// </summary>
    public string FullName => string.Join(" ", new[] { FirstName, LastName }.Where(part => !string.IsNullOrWhiteSpace(part)));
}
