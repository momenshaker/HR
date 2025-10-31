using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.Common;
using HR.Application.Common.Exceptions;
using HR.Application.DTOs;
using HR.Application.Mappings;
using HR.Domain.Entities;

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
            .OrderBy(dto => dto.Path, StringComparer.Ordinal)
            .ToArray();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<DepartmentDto>> GetHierarchyAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        var departments = await _departmentRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var scopedDepartments = departments
            .Where(department => department.OrganizationId == organizationId)
            .OrderBy(department => department.Path, StringComparer.Ordinal)
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
            .OrderBy(node => node.Department.Path, StringComparer.Ordinal)
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
        var parent = await EnsureValidParentAsync(
                organizationId,
                normalizedRequest.ParentDepartmentId,
                currentDepartmentId: null,
                nameof(CreateDepartmentRequest.ParentDepartmentId),
                cancellationToken)
            .ConfigureAwait(false);

        await EnsureDepartmentIsUniqueAsync(
                organizationId,
                normalizedRequest.ParentDepartmentId,
                normalizedRequest.Name,
                normalizedRequest.Code,
                excludingDepartmentId: null,
                cancellationToken)
            .ConfigureAwait(false);

        var departmentId = Guid.NewGuid();
        var level = parent is null ? 0 : parent.Level + 1;
        var path = DepartmentHierarchyPath.Build(organizationId, departmentId, parent?.Path);
        var entity = normalizedRequest.ToEntity(departmentId, path, level, DateTime.UtcNow);
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
        await EnsureValidParentAsync(
                organizationId,
                normalizedRequest.ParentDepartmentId,
                existing.Id,
                nameof(UpdateDepartmentRequest.ParentDepartmentId),
                cancellationToken)
            .ConfigureAwait(false);

        if (normalizedRequest.ParentDepartmentId != existing.ParentDepartmentId)
        {
            throw CreateValidationException(
                nameof(UpdateDepartmentRequest.ParentDepartmentId),
                "Use the move endpoint to change the department parent.",
                "HierarchyMutation");
        }

        await EnsureDepartmentIsUniqueAsync(
                organizationId,
                normalizedRequest.ParentDepartmentId,
                normalizedRequest.Name,
                normalizedRequest.Code,
                existing.Id,
                cancellationToken)
            .ConfigureAwait(false);

        var updatedEntity = normalizedRequest.ApplyUpdates(existing, existing.Path, existing.Level);
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

        await EnsureValidParentAsync(
                organizationId,
                newParentDepartmentId,
                department.Id,
                nameof(MoveDepartmentRequest.NewParentDepartmentId),
                cancellationToken)
            .ConfigureAwait(false);

        await EnsureDepartmentIsUniqueAsync(
                organizationId,
                newParentDepartmentId,
                department.Name,
                department.Code,
                department.Id,
                cancellationToken)
            .ConfigureAwait(false);

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
            .OrderBy(dto => dto.Path, StringComparer.Ordinal)
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

    private static ValidationException CreateValidationException(
        string propertyName,
        string message,
        string errorCode)
    {
        var failure = new ValidationFailure(propertyName, message)
        {
            ErrorCode = errorCode
        };

        return new ValidationException(new[] { failure });
    }

    private async Task<Department?> EnsureValidParentAsync(
        Guid organizationId,
        Guid? parentDepartmentId,
        Guid? currentDepartmentId,
        string propertyName,
        CancellationToken cancellationToken)
    {
        if (!parentDepartmentId.HasValue)
        {
            return null;
        }

        var parent = await _departmentRepository
            .GetByIdAsync(parentDepartmentId.Value, cancellationToken)
            .ConfigureAwait(false);

        if (parent is null || parent.OrganizationId != organizationId)
        {
            throw CreateValidationException(
                propertyName,
                "The specified parent department must belong to the same organization.",
                "OrgMismatch");
        }

        if (!currentDepartmentId.HasValue)
        {
            return parent;
        }

        if (parentDepartmentId.Value == currentDepartmentId.Value)
        {
            throw CreateValidationException(
                propertyName,
                "A department cannot be assigned as its own parent.",
                "HierarchyCycle");
        }

        var subtree = await _departmentTreeService
            .GetSubtreeAsync(currentDepartmentId.Value, cancellationToken)
            .ConfigureAwait(false);

        if (subtree.Skip(1).Any(node => node.Id == parentDepartmentId.Value))
        {
            throw CreateValidationException(
                propertyName,
                "Assigning the selected parent would create a hierarchy cycle.",
                "HierarchyCycle");
        }

        return parent;
    }

    private async Task EnsureDepartmentIsUniqueAsync(
        Guid organizationId,
        Guid? parentDepartmentId,
        string name,
        string code,
        Guid? excludingDepartmentId,
        CancellationToken cancellationToken)
    {
        var normalizedName = name.Trim();
        var normalizedCode = code.Trim().ToUpperInvariant();

        var nameExists = await _departmentRepository
            .ExistsByNameAsync(
                organizationId,
                parentDepartmentId,
                normalizedName,
                excludingDepartmentId,
                cancellationToken)
            .ConfigureAwait(false);

        if (nameExists)
        {
            throw new UniqueConstraintViolationException("Department", "Name", normalizedName);
        }

        if (string.IsNullOrWhiteSpace(normalizedCode))
        {
            return;
        }

        var codeExists = await _departmentRepository
            .ExistsByCodeAsync(
                organizationId,
                normalizedCode,
                excludingDepartmentId,
                cancellationToken)
            .ConfigureAwait(false);

        if (codeExists)
        {
            throw new UniqueConstraintViolationException("Department", "Code", normalizedCode);
        }
    }

    private sealed class DepartmentHierarchyBuilder
    {
        public DepartmentHierarchyBuilder(DepartmentDto department)
        {
            Department = department;
        }

        public DepartmentDto Department { get; }

        public List<DepartmentHierarchyBuilder> Children { get; } = new();

        public DepartmentDto ToDto()
        {
            var orderedChildren = Children
                .OrderBy(child => child.Department.Path, StringComparer.Ordinal)
                .Select(child => child.ToDto())
                .ToArray();

            return Department with { Children = orderedChildren };
        }
    }
}
