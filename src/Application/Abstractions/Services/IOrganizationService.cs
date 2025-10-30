using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service orchestrating organization operations.
/// </summary>
public interface IOrganizationService
{
    Task<IReadOnlyCollection<OrganizationDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<OrganizationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<OrganizationDto> CreateAsync(CreateOrganizationRequest request, CancellationToken cancellationToken = default);

    Task<OrganizationDto?> UpdateAsync(Guid id, UpdateOrganizationRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
