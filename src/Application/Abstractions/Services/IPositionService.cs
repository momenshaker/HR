using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service exposing organisation position management.
/// </summary>
public interface IPositionService
{
    Task<IReadOnlyCollection<PositionDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<PositionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PositionDto>> GetByOrganizationUnitAsync(Guid organizationUnitId, CancellationToken cancellationToken = default);

    Task<PositionDto?> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<PositionDto> CreateAsync(CreatePositionRequest request, CancellationToken cancellationToken = default);

    Task<PositionDto?> UpdateAsync(Guid id, UpdatePositionRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
