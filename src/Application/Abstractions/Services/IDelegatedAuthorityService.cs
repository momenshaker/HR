using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service exposing delegated authority workflows.
/// </summary>
public interface IDelegatedAuthorityService
{
    Task<IReadOnlyCollection<DelegatedAuthorityDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<DelegatedAuthorityDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DelegatedAuthorityDto>> GetByGrantorAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DelegatedAuthorityDto>> GetByDelegateAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<DelegatedAuthorityDto> CreateAsync(CreateDelegatedAuthorityRequest request, CancellationToken cancellationToken = default);

    Task<DelegatedAuthorityDto?> UpdateAsync(Guid id, UpdateDelegatedAuthorityRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
