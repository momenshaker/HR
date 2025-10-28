using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service exposing organisation unit operations.
/// </summary>
public interface IOrganizationUnitService
{
    Task<IReadOnlyCollection<OrganizationUnitDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<OrganizationUnitDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<OrganizationHierarchyNodeDto>> GetHierarchyAsync(CancellationToken cancellationToken = default);

    Task<OrganizationUnitDto> CreateAsync(CreateOrganizationUnitRequest request, CancellationToken cancellationToken = default);

    Task<OrganizationUnitDto?> UpdateAsync(Guid id, UpdateOrganizationUnitRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
