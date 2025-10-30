using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class OrganizationService : IOrganizationService
{
    private readonly IOrganizationRepository _organizationRepository;

    public OrganizationService(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<OrganizationDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var organizations = await _organizationRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return organizations.Select(organization => organization.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<OrganizationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var organization = await _organizationRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return organization?.ToDto();
    }

    /// <inheritdoc />
    public async Task<OrganizationDto> CreateAsync(CreateOrganizationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = request.ToEntity();
        var created = await _organizationRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<OrganizationDto?> UpdateAsync(Guid id, UpdateOrganizationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _organizationRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updatedEntity = request.ApplyUpdates(existing);
        var persisted = await _organizationRepository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);

        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _organizationRepository.RemoveAsync(id, cancellationToken);
    }
}
