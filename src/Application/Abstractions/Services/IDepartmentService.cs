using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service responsible for orchestrating department use cases.
/// </summary>
public interface IDepartmentService
{
    Task<IReadOnlyCollection<DepartmentDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<DepartmentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<DepartmentDto> CreateAsync(CreateDepartmentRequest request, CancellationToken cancellationToken = default);

    Task<DepartmentDto?> UpdateAsync(Guid id, UpdateDepartmentRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
