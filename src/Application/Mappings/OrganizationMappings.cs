using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="Organization" /> entities.
/// </summary>
public static class OrganizationMappings
{
    public static OrganizationDto ToDto(this Organization organization)
    {
        ArgumentNullException.ThrowIfNull(organization);

        return new OrganizationDto(
            organization.Id,
            organization.Name,
            organization.Code,
            organization.Description,
            organization.IsActive);
    }

    public static Organization ToEntity(this CreateOrganizationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new Organization
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Code = request.Code.Trim().ToUpperInvariant(),
            Description = request.Description.Trim(),
            IsActive = request.IsActive
        };
    }

    public static Organization ApplyUpdates(this UpdateOrganizationRequest request, Organization existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        return new Organization
        {
            Id = existing.Id,
            Name = request.Name.Trim(),
            Code = request.Code.Trim().ToUpperInvariant(),
            Description = request.Description.Trim(),
            IsActive = request.IsActive
        };
    }
}
