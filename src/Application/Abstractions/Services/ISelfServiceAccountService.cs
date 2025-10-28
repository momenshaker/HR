using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service exposing employee self-service account operations.
/// </summary>
public interface ISelfServiceAccountService
{
    Task<IReadOnlyCollection<SelfServiceAccountDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<SelfServiceAccountDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<SelfServiceAccountDto?> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<SelfServiceAccountDto> CreateAsync(CreateSelfServiceAccountRequest request, CancellationToken cancellationToken = default);

    Task<SelfServiceAccountDto?> UpdateAsync(Guid id, UpdateSelfServiceAccountRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
