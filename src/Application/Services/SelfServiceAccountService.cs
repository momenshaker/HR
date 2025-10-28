using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class SelfServiceAccountService : ISelfServiceAccountService
{
    private readonly ISelfServiceAccountRepository _repository;

    public SelfServiceAccountService(ISelfServiceAccountRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<SelfServiceAccountDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await _repository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return accounts.Select(account => account.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<SelfServiceAccountDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var account = await _repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return account?.ToDto();
    }

    /// <inheritdoc />
    public async Task<SelfServiceAccountDto?> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var account = await _repository.GetByEmployeeIdAsync(employeeId, cancellationToken).ConfigureAwait(false);
        return account?.ToDto();
    }

    /// <inheritdoc />
    public async Task<SelfServiceAccountDto> CreateAsync(
        CreateSelfServiceAccountRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _repository.GetByEmployeeIdAsync(request.EmployeeId, cancellationToken).ConfigureAwait(false);
        if (existing is not null)
        {
            throw new InvalidOperationException("A self-service account already exists for the specified employee.");
        }

        var entity = request.ToEntity();
        var created = await _repository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<SelfServiceAccountDto?> UpdateAsync(
        Guid id,
        UpdateSelfServiceAccountRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updatedEntity = request.ApplyUpdates(existing);
        var persisted = await _repository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);
        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _repository.RemoveAsync(id, cancellationToken);
    }
}
