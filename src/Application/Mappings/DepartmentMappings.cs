using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="Department" /> entities.
/// </summary>
public static class DepartmentMappings
{
    public static DepartmentDto ToDto(this Department department)
    {
        ArgumentNullException.ThrowIfNull(department);

        return new DepartmentDto(
            department.Id,
            department.Name,
            department.Code,
            department.ParentDepartmentId,
            department.ManagerId,
            department.Branch,
            department.Location,
            department.Description,
            department.IsActive);
    }

    public static Department ToEntity(this CreateDepartmentRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new Department
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Code = request.Code.Trim().ToUpperInvariant(),
            ParentDepartmentId = request.ParentDepartmentId,
            ManagerId = request.ManagerId,
            Branch = request.Branch.Trim(),
            Location = request.Location.Trim(),
            Description = request.Description.Trim(),
            IsActive = request.IsActive
        };
    }

    public static Department ApplyUpdates(this UpdateDepartmentRequest request, Department existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        return new Department
        {
            Id = existing.Id,
            Name = request.Name.Trim(),
            Code = request.Code.Trim().ToUpperInvariant(),
            ParentDepartmentId = request.ParentDepartmentId,
            ManagerId = request.ManagerId,
            Branch = request.Branch.Trim(),
            Location = request.Location.Trim(),
            Description = request.Description.Trim(),
            IsActive = request.IsActive
        };
    }
}
