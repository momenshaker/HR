using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class DelegatedAuthorityService : IDelegatedAuthorityService
{
    private readonly IDelegatedAuthorityRepository _delegatedAuthorityRepository;

    public DelegatedAuthorityService(IDelegatedAuthorityRepository delegatedAuthorityRepository)
    {
        _delegatedAuthorityRepository = delegatedAuthorityRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<DelegatedAuthorityDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var authorities = await _delegatedAuthorityRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return authorities.Select(authority => authority.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<DelegatedAuthorityDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var authority = await _delegatedAuthorityRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return authority?.ToDto();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<DelegatedAuthorityDto>> GetByGrantorAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        var authorities = await _delegatedAuthorityRepository
            .GetByGrantorAsync(employeeId, cancellationToken)
            .ConfigureAwait(false);

        return authorities.Select(authority => authority.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<DelegatedAuthorityDto>> GetByDelegateAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        var authorities = await _delegatedAuthorityRepository
            .GetByDelegateAsync(employeeId, cancellationToken)
            .ConfigureAwait(false);

        return authorities.Select(authority => authority.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<DelegatedAuthorityDto> CreateAsync(
        CreateDelegatedAuthorityRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = request.ToEntity();
        var created = await _delegatedAuthorityRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<DelegatedAuthorityDto?> UpdateAsync(
        Guid id,
        UpdateDelegatedAuthorityRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _delegatedAuthorityRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updatedEntity = request.ApplyUpdates(existing);
        var persisted = await _delegatedAuthorityRepository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);
        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _delegatedAuthorityRepository.RemoveAsync(id, cancellationToken);
    }
}
