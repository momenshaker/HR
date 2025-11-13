using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service for managing dynamic lookup values.
/// </summary>
public interface ILookupService
{
    Task<LookupCollectionDto> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<LookupValueDto>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);

    Task<LookupValueDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<LookupValueDto> CreateAsync(CreateLookupValueRequest request, CancellationToken cancellationToken = default);

    Task<LookupValueDto?> UpdateAsync(Guid id, UpdateLookupValueRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
