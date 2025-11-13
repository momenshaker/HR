using System;
using System.Collections.Generic;
using System.Linq;

namespace HR.Domain.Entities;

/// <summary>
///     Represents an employee within the organization including master data enrichments.
/// </summary>
public sealed class Employee
{
    public Guid Id { get; init; }

    public string Email { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string JobTitle { get; init; } = string.Empty;

    public string PhoneNumber { get; init; } = string.Empty;

    public string EmploymentType { get; init; } = string.Empty;

    public DateOnly EmploymentStartDate { get; init; }

    public DateOnly? EmploymentEndDate { get; init; }

    public DateOnly? DateOfBirth { get; init; }

    public bool IsActive { get; init; } = true;

    public DateTime CreatedAtUtc { get; init; }

    public EmployeeJobArchitecture JobArchitecture { get; init; } = EmployeeJobArchitecture.Empty;

    public ICollection<EmployeeDepartment> Departments { get; init; } = new List<EmployeeDepartment>();

    public ICollection<EmploymentContract> Contracts { get; init; } = new List<EmploymentContract>();

    public ICollection<EmployeeComplianceDocument> ComplianceDocuments { get; init; } = new List<EmployeeComplianceDocument>();

    public ICollection<EmployeeProfileDocument> ProfileDocuments { get; init; } = new List<EmployeeProfileDocument>();

    public string FullName => string.Join(" ", new[] { FirstName, LastName }.Where(part => !string.IsNullOrWhiteSpace(part)));

    public IReadOnlyCollection<Guid> DepartmentIds => Departments
        .Select(membership => membership.DepartmentId)
        .ToArray();

    public Guid? PrimaryDepartmentId => Departments
        .FirstOrDefault(membership => membership.IsPrimary)
        ?.DepartmentId;
}
