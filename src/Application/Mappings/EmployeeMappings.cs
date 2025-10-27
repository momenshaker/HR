using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Helper methods for transforming <see cref="Employee" /> entities into transport-friendly representations.
/// </summary>
public static class EmployeeMappings
{
    public static EmployeeDto ToDto(this Employee employee)
    {
        ArgumentNullException.ThrowIfNull(employee);

        return new EmployeeDto(
            employee.Id,
            employee.FirstName,
            employee.LastName,
            employee.Email,
            employee.JobTitle,
            employee.DepartmentId,
            employee.EmploymentStartDate,
            employee.EmploymentEndDate,
            employee.DateOfBirth);
    }

    public static Employee ToEntity(this CreateEmployeeRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            JobTitle = request.JobTitle.Trim(),
            DepartmentId = request.DepartmentId,
            EmploymentStartDate = request.EmploymentStartDate,
            EmploymentEndDate = request.EmploymentEndDate,
            DateOfBirth = request.DateOfBirth
        };
    }

    public static Employee ApplyUpdates(this UpdateEmployeeRequest request, Employee existingEmployee)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existingEmployee);

        return new Employee
        {
            Id = existingEmployee.Id,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            JobTitle = request.JobTitle.Trim(),
            DepartmentId = request.DepartmentId,
            EmploymentStartDate = request.EmploymentStartDate,
            EmploymentEndDate = request.EmploymentEndDate,
            DateOfBirth = request.DateOfBirth
        };
    }
}
