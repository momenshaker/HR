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

        return new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            Code = department.Code ?? string.Empty,
            OrganizationId = department.OrganizationId,
            ParentDepartmentId = department.ParentDepartmentId,
            ManagerId = department.ManagerId,
            Branch = department.Branch ?? string.Empty,
            Location = department.Location ?? string.Empty,
            Description = department.Description ?? string.Empty,
            IsActive = department.IsActive,
            Children = Array.Empty<DepartmentDto>()
        };
    }

    public static Department ToEntity(this CreateDepartmentRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new Department
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Code = request.Code.Trim().ToUpperInvariant(),
            OrganizationId = request.OrganizationId,
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
            OrganizationId = request.OrganizationId,
            ParentDepartmentId = request.ParentDepartmentId,
            ManagerId = request.ManagerId,
            Branch = request.Branch.Trim(),
            Location = request.Location.Trim(),
            Description = request.Description.Trim(),
            IsActive = request.IsActive
        };
    }
}
