using System.ComponentModel.DataAnnotations;
using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IDepartmentTreeService _departmentTreeService;
    private readonly IEmployeeDepartmentRepository _employeeDepartmentRepository;

    public DepartmentService(
        IDepartmentRepository departmentRepository,
        IDepartmentTreeService departmentTreeService,
        IEmployeeDepartmentRepository employeeDepartmentRepository)
    {
        _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
        _departmentTreeService = departmentTreeService ?? throw new ArgumentNullException(nameof(departmentTreeService));
        _employeeDepartmentRepository = employeeDepartmentRepository
            ?? throw new ArgumentNullException(nameof(employeeDepartmentRepository));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<DepartmentDto>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        var departments = await _departmentRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);

        return departments
            .Where(department => department.OrganizationId == organizationId)
            .Select(department => department.ToDto())
            .OrderBy(dto => dto.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<DepartmentHierarchyDto>> GetHierarchyAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        var departments = await _departmentRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var scopedDepartments = departments
            .Where(department => department.OrganizationId == organizationId)
            .ToArray();

        var builders = scopedDepartments.ToDictionary(
            department => department.Id,
            department => new DepartmentHierarchyBuilder(department.ToDto()));

        var roots = new List<DepartmentHierarchyBuilder>();

        foreach (var department in scopedDepartments)
        {
            var builder = builders[department.Id];

            if (department.ParentDepartmentId.HasValue &&
                builders.TryGetValue(department.ParentDepartmentId.Value, out var parentBuilder))
            {
                parentBuilder.Children.Add(builder);
            }
            else
            {
                roots.Add(builder);
            }
        }

        return roots
            .OrderBy(node => node.Department.Name, StringComparer.OrdinalIgnoreCase)
            .Select(node => node.ToDto())
            .ToArray();
    }

    /// <inheritdoc />
    public async Task<DepartmentDto?> GetByIdAsync(
        Guid organizationId,
        Guid departmentId,
        CancellationToken cancellationToken = default)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentId, cancellationToken).ConfigureAwait(false);
        if (department is null || department.OrganizationId != organizationId)
        {
            return null;
        }

        return department.ToDto();
    }

    /// <inheritdoc />
    public async Task<DepartmentDto> CreateAsync(
        Guid organizationId,
        CreateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var normalizedRequest = NormalizeCreateRequest(organizationId, request);
        var entity = normalizedRequest.ToEntity();
        var created = await _departmentRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<DepartmentDto?> UpdateAsync(
        Guid organizationId,
        Guid departmentId,
        UpdateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _departmentRepository.GetByIdAsync(departmentId, cancellationToken).ConfigureAwait(false);
        if (existing is null || existing.OrganizationId != organizationId)
        {
            return null;
        }

        var normalizedRequest = NormalizeUpdateRequest(organizationId, request);
        var updatedEntity = normalizedRequest.ApplyUpdates(existing);
        var persisted = await _departmentRepository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);

        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public async Task<DepartmentDto?> MoveAsync(
        Guid organizationId,
        Guid departmentId,
        Guid? newParentDepartmentId,
        CancellationToken cancellationToken = default)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentId, cancellationToken).ConfigureAwait(false);
        if (department is null || department.OrganizationId != organizationId)
        {
            return null;
        }

        if (newParentDepartmentId.HasValue)
        {
            var parent = await _departmentRepository
                .GetByIdAsync(newParentDepartmentId.Value, cancellationToken)
                .ConfigureAwait(false);

            if (parent is null || parent.OrganizationId != organizationId)
            {
                throw new ValidationException("The new parent department must exist within the same organization.");
            }
        }

        await _departmentTreeService
            .MoveDepartmentAsync(departmentId, newParentDepartmentId, cancellationToken)
            .ConfigureAwait(false);

        var updated = await _departmentRepository.GetByIdAsync(departmentId, cancellationToken).ConfigureAwait(false);
        return updated?.ToDto();
    }

    /// <inheritdoc />
    public async Task<DepartmentDeleteResult> DeleteAsync(
        Guid organizationId,
        Guid departmentId,
        bool cascade,
        CancellationToken cancellationToken = default)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentId, cancellationToken).ConfigureAwait(false);
        if (department is null || department.OrganizationId != organizationId)
        {
            return DepartmentDeleteResult.Missing();
        }

        var departments = await _departmentRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var hasChildren = departments.Any(child => child.ParentDepartmentId == departmentId);

        if (hasChildren && !cascade)
        {
            return DepartmentDeleteResult.Blocked();
        }

        if (!cascade)
        {
            var deleted = await _departmentRepository.RemoveAsync(departmentId, cancellationToken).ConfigureAwait(false);
            return deleted ? DepartmentDeleteResult.Success(1) : DepartmentDeleteResult.Missing();
        }

        var subtree = await _departmentTreeService
            .GetSubtreeAsync(departmentId, cancellationToken)
            .ConfigureAwait(false);

        var deletedCount = 0;
        foreach (var node in subtree)
        {
            if (node.OrganizationId != organizationId)
            {
                continue;
            }

            if (await _departmentRepository.RemoveAsync(node.Id, cancellationToken).ConfigureAwait(false))
            {
                deletedCount++;
            }
        }

        return deletedCount > 0
            ? DepartmentDeleteResult.Success(deletedCount)
            : DepartmentDeleteResult.Missing();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<DepartmentDto>> GetByEmployeeAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        var departmentIds = await _employeeDepartmentRepository
            .GetDepartmentIdsByEmployeeAsync(employeeId, cancellationToken)
            .ConfigureAwait(false);

        if (departmentIds.Count == 0)
        {
            return Array.Empty<DepartmentDto>();
        }

        var departments = await _departmentRepository
            .GetByIdsAsync(departmentIds, cancellationToken)
            .ConfigureAwait(false);

        return departments
            .Select(department => department.ToDto())
            .OrderBy(dto => dto.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static CreateDepartmentRequest NormalizeCreateRequest(
        Guid organizationId,
        CreateDepartmentRequest request)
    {
        if (request.OrganizationId != Guid.Empty && request.OrganizationId != organizationId)
        {
            throw new ValidationException("OrganizationId in the payload must match the route parameter.");
        }

        if (request.OrganizationId == organizationId)
        {
            return request;
        }

        return new CreateDepartmentRequest
        {
            Name = request.Name,
            Code = request.Code,
            OrganizationId = organizationId,
            ParentDepartmentId = request.ParentDepartmentId,
            ManagerId = request.ManagerId,
            Branch = request.Branch,
            Location = request.Location,
            Description = request.Description,
            IsActive = request.IsActive
        };
    }

    private static UpdateDepartmentRequest NormalizeUpdateRequest(
        Guid organizationId,
        UpdateDepartmentRequest request)
    {
        if (request.OrganizationId != Guid.Empty && request.OrganizationId != organizationId)
        {
            throw new ValidationException("OrganizationId in the payload must match the route parameter.");
        }

        if (request.OrganizationId == organizationId)
        {
            return request;
        }

        return new UpdateDepartmentRequest
        {
            Name = request.Name,
            Code = request.Code,
            OrganizationId = organizationId,
            ParentDepartmentId = request.ParentDepartmentId,
            ManagerId = request.ManagerId,
            Branch = request.Branch,
            Location = request.Location,
            Description = request.Description,
            IsActive = request.IsActive
        };
    }

    private sealed class DepartmentHierarchyBuilder
    {
        public DepartmentHierarchyBuilder(DepartmentDto department)
        {
            Department = department;
        }

        public DepartmentDto Department { get; }

        public List<DepartmentHierarchyBuilder> Children { get; } = new();

        public DepartmentHierarchyDto ToDto()
        {
            var orderedChildren = Children
                .OrderBy(child => child.Department.Name, StringComparer.OrdinalIgnoreCase)
                .Select(child => child.ToDto())
                .ToArray();

            return new DepartmentHierarchyDto(Department, orderedChildren);
        }
    }
}
