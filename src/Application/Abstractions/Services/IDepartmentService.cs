using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service responsible for orchestrating department use cases.
/// </summary>
public interface IDepartmentService
{
    Task<IReadOnlyCollection<DepartmentDto>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DepartmentDto>> GetHierarchyAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<DepartmentDto?> GetByIdAsync(
        Guid organizationId,
        Guid departmentId,
        CancellationToken cancellationToken = default);

    Task<DepartmentDto> CreateAsync(
        Guid organizationId,
        CreateDepartmentRequest request,
        CancellationToken cancellationToken = default);

    Task<DepartmentDto?> UpdateAsync(
        Guid organizationId,
        Guid departmentId,
        UpdateDepartmentRequest request,
        CancellationToken cancellationToken = default);

    Task<DepartmentDto?> MoveAsync(
        Guid organizationId,
        Guid departmentId,
        Guid? newParentDepartmentId,
        CancellationToken cancellationToken = default);

    Task<DepartmentDeleteResult> DeleteAsync(
        Guid organizationId,
        Guid departmentId,
        bool cascade,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DepartmentDto>> GetByEmployeeAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);
}
