using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="OrganizationUnit" /> entities.
/// </summary>
public static class OrganizationUnitMappings
{
    public static OrganizationUnitDto ToDto(this OrganizationUnit unit)
    {
        ArgumentNullException.ThrowIfNull(unit);

        return new OrganizationUnitDto(
            unit.Id,
            unit.Name,
            unit.Code,
            unit.Type,
            unit.ParentUnitId,
            unit.DepartmentId,
            unit.LeadPositionId,
            unit.Level,
            unit.Description,
            unit.IsActive);
    }

    public static OrganizationUnit ToEntity(this CreateOrganizationUnitRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new OrganizationUnit
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Code = request.Code.Trim().ToUpperInvariant(),
            Type = request.Type.Trim(),
            ParentUnitId = request.ParentUnitId,
            DepartmentId = request.DepartmentId,
            LeadPositionId = request.LeadPositionId,
            Level = request.Level,
            Description = request.Description.Trim(),
            IsActive = request.IsActive
        };
    }

    public static OrganizationUnit ApplyUpdates(this UpdateOrganizationUnitRequest request, OrganizationUnit existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        return new OrganizationUnit
        {
            Id = existing.Id,
            Name = request.Name.Trim(),
            Code = request.Code.Trim().ToUpperInvariant(),
            Type = request.Type.Trim(),
            ParentUnitId = request.ParentUnitId,
            DepartmentId = request.DepartmentId,
            LeadPositionId = request.LeadPositionId,
            Level = request.Level,
            Description = request.Description.Trim(),
            IsActive = request.IsActive
        };
    }
}
