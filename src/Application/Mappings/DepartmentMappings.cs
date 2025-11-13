using System;
using HR.Application.Common;
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
            Code = department.Code,
            OrganizationId = department.OrganizationId,
            ParentDepartmentId = department.ParentDepartmentId,
            ManagerId = department.ManagerId,
            Branch = department.Branch,
            Location = department.Location,
            BusinessUnit = department.BusinessUnit,
            CostCenterCode = department.CostCenterCode,
            OperatingHours = department.OperatingHours,
            BudgetOwner = department.BudgetOwner,
            Description = department.Description,
            IsActive = department.IsActive,
            Path = department.Path,
            Level = department.Level,
            Children = Array.Empty<DepartmentDto>()
        };
    }

    public static Department ToEntity(
        this CreateDepartmentRequest request,
        Guid departmentId,
        string path,
        int level,
        DateTime createdAtUtc)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new Department
        {
            Id = departmentId,
            Name = request.Name.Trim(),
            Code = request.Code.Trim().ToUpperInvariant(),
            OrganizationId = request.OrganizationId,
            ParentDepartmentId = request.ParentDepartmentId,
            ManagerId = request.ManagerId,
            Branch = request.Branch.Trim(),
            Location = request.Location.Trim(),
            BusinessUnit = request.BusinessUnit.Trim(),
            CostCenterCode = request.CostCenterCode.Trim(),
            OperatingHours = request.OperatingHours.Trim(),
            BudgetOwner = request.BudgetOwner.Trim(),
            Description = request.Description.Trim(),
            IsActive = request.IsActive,
            Path = path,
            Level = level,
            CreatedAtUtc = createdAtUtc
        };
    }

    public static Department ApplyUpdates(
        this UpdateDepartmentRequest request,
        Department existing,
        string path,
        int level)
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
            BusinessUnit = request.BusinessUnit.Trim(),
            CostCenterCode = request.CostCenterCode.Trim(),
            OperatingHours = request.OperatingHours.Trim(),
            BudgetOwner = request.BudgetOwner.Trim(),
            Description = request.Description.Trim(),
            IsActive = request.IsActive,
            Path = path,
            Level = level,
            CreatedAtUtc = existing.CreatedAtUtc,
            UpdatedAtUtc = DateTime.UtcNow
        };
    }
}
