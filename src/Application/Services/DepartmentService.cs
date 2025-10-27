using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;

    public DepartmentService(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<DepartmentDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var departments = await _departmentRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return departments.Select(department => department.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<DepartmentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var department = await _departmentRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return department?.ToDto();
    }

    /// <inheritdoc />
    public async Task<DepartmentDto> CreateAsync(CreateDepartmentRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = request.ToEntity();
        var created = await _departmentRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<DepartmentDto?> UpdateAsync(Guid id, UpdateDepartmentRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _departmentRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updatedEntity = request.ApplyUpdates(existing);
        var persisted = await _departmentRepository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);

        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _departmentRepository.RemoveAsync(id, cancellationToken);
    }
}
