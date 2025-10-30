using System;

namespace HR.Domain.Entities;

/// <summary>
///     Join entity linking employees to the departments they belong to.
/// </summary>
public sealed class EmployeeDepartment
{
    public EmployeeDepartment(Guid employeeId, Guid departmentId)
    {
        EmployeeId = employeeId;
        DepartmentId = departmentId;
    }

    public Guid EmployeeId { get; }

    public Guid DepartmentId { get; }

    public Employee? Employee { get; private set; }

    public Department? Department { get; private set; }
}
