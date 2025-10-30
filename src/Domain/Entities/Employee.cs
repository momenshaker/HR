using System;
using System.Collections.Generic;

namespace HR.Domain.Entities;

/// <summary>
///     Represents an employee within the organization including master data enrichments.
/// </summary>
public sealed class Employee
{
    public Employee(Guid id, string email, string firstName, string lastName, DateTime createdAtUtc, bool isActive = true)
    {
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        CreatedAtUtc = createdAtUtc;
        IsActive = isActive;
    }

    public Guid Id { get; }

    public string Email { get; }

    public string FirstName { get; }

    public string LastName { get; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; }

    public ICollection<EmployeeDepartment> EmployeeDepartments { get; } = new List<EmployeeDepartment>();
}
