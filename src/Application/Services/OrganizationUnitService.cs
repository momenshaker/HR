using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class OrganizationUnitService : IOrganizationUnitService
{
    private readonly IOrganizationUnitRepository _organizationUnitRepository;

    public OrganizationUnitService(IOrganizationUnitRepository organizationUnitRepository)
    {
        _organizationUnitRepository = organizationUnitRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<OrganizationUnitDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var units = await _organizationUnitRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return units.Select(unit => unit.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<OrganizationUnitDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var unit = await _organizationUnitRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return unit?.ToDto();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<OrganizationHierarchyNodeDto>> GetHierarchyAsync(CancellationToken cancellationToken = default)
    {
        var units = await _organizationUnitRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var unitDtos = units.Select(unit => unit.ToDto()).ToArray();
        var lookup = unitDtos.ToLookup(unit => unit.ParentUnitId);

        IReadOnlyCollection<OrganizationHierarchyNodeDto> Build(Guid? parentId)
        {
            return lookup[parentId]
                .Select(child => new OrganizationHierarchyNodeDto(child, Build(child.Id)))
                .ToArray();
        }

        return Build(null);
    }

    /// <inheritdoc />
    public async Task<OrganizationUnitDto> CreateAsync(
        CreateOrganizationUnitRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = request.ToEntity();
        var created = await _organizationUnitRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<OrganizationUnitDto?> UpdateAsync(
        Guid id,
        UpdateOrganizationUnitRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _organizationUnitRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updatedEntity = request.ApplyUpdates(existing);
        var persisted = await _organizationUnitRepository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);
        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _organizationUnitRepository.RemoveAsync(id, cancellationToken);
    }
}
