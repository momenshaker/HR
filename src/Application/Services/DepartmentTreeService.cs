using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Domain.Entities;

namespace HR.Application.Services;

/// <summary>
///     Provides operations for navigating and restructuring the department hierarchy.
/// </summary>
public sealed class DepartmentTreeService : IDepartmentTreeService
{
    private readonly IDepartmentRepository _departmentRepository;

    public DepartmentTreeService(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
    }

    /// <inheritdoc />
    public async Task MoveDepartmentAsync(
        Guid departmentId,
        Guid? newParentDepartmentId = null,
        CancellationToken cancellationToken = default)
    {
        var departments = await _departmentRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var departmentsById = departments.ToDictionary(department => department.Id);

        if (!departmentsById.TryGetValue(departmentId, out var department))
        {
            throw new InvalidOperationException($"Department '{departmentId}' was not found.");
        }

        Department? newParent = null;
        if (newParentDepartmentId.HasValue)
        {
            if (!departmentsById.TryGetValue(newParentDepartmentId.Value, out newParent))
            {
                throw new InvalidOperationException($"Parent department '{newParentDepartmentId}' was not found.");
            }
        }

        if (newParent is not null && newParent.OrganizationId != department.OrganizationId)
        {
            throw new InvalidOperationException("Departments can only be moved within the same organization.");
        }

        var childrenLookup = BuildChildrenLookup(departments);
        var descendantIds = CollectDescendantIds(department.Id, childrenLookup);

        if (newParentDepartmentId.HasValue)
        {
            if (newParentDepartmentId.Value == department.Id)
            {
                throw new InvalidOperationException("A department cannot be set as its own parent.");
            }

            if (descendantIds.Contains(newParentDepartmentId.Value))
            {
                throw new InvalidOperationException("Moving the department under the selected parent would create a cycle.");
            }
        }

        var parentPath = newParent is null ? GetRootPathPrefix(department.OrganizationId) : newParent.Path;
        var newLevel = newParent is null ? 0 : newParent.Level + 1;
        var updatedDepartments = new List<Department>();

        var updatedRoot = department.WithHierarchy(newParentDepartmentId, newLevel, $"{parentPath}/{department.Id}");
        updatedDepartments.Add(updatedRoot);

        var queue = new Queue<Department>();
        queue.Enqueue(updatedRoot);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (!childrenLookup.TryGetValue(current.Id, out var children))
            {
                continue;
            }

            foreach (var child in children)
            {
                if (child.Id == department.Id)
                {
                    continue;
                }

                var updatedChild = child.WithHierarchy(child.ParentDepartmentId, current.Level + 1, $"{current.Path}/{child.Id}");
                updatedDepartments.Add(updatedChild);
                queue.Enqueue(updatedChild);
            }
        }

        foreach (var updated in updatedDepartments)
        {
            await _departmentRepository.UpdateAsync(updated, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Department>> GetSubtreeAsync(
        Guid rootDepartmentId,
        CancellationToken cancellationToken = default)
    {
        var departments = await _departmentRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var departmentsById = departments.ToDictionary(department => department.Id);

        if (!departmentsById.TryGetValue(rootDepartmentId, out var root))
        {
            throw new InvalidOperationException($"Department '{rootDepartmentId}' was not found.");
        }

        var childrenLookup = BuildChildrenLookup(departments);
        var ordered = new List<Department>();
        var queue = new Queue<Department>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            ordered.Add(current);

            if (!childrenLookup.TryGetValue(current.Id, out var children))
            {
                continue;
            }

            foreach (var child in children.OrderBy(child => child.Path, StringComparer.Ordinal))
            {
                queue.Enqueue(child);
            }
        }

        return ordered
            .OrderBy(department => department.Path, StringComparer.Ordinal)
            .ToArray();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Department>> GetAncestorsAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default)
    {
        var (_, ancestors) = await GetDepartmentAndAncestorsAsync(departmentId, cancellationToken).ConfigureAwait(false);
        return ancestors;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Department>> GetBreadcrumbAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default)
    {
        var (department, ancestors) = await GetDepartmentAndAncestorsAsync(departmentId, cancellationToken).ConfigureAwait(false);
        return ancestors.Concat(new[] { department }).ToArray();
    }

    private async Task<(Department Department, IReadOnlyCollection<Department> Ancestors)> GetDepartmentAndAncestorsAsync(
        Guid departmentId,
        CancellationToken cancellationToken)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentId, cancellationToken).ConfigureAwait(false)
                        ?? throw new InvalidOperationException($"Department '{departmentId}' was not found.");

        var ancestors = new List<Department>();
        var visited = new HashSet<Guid> { department.Id };
        var nextParentId = department.ParentDepartmentId;

        while (nextParentId.HasValue)
        {
            if (!visited.Add(nextParentId.Value))
            {
                throw new InvalidOperationException("A cycle was detected while traversing department ancestors.");
            }

            var parent = await _departmentRepository.GetByIdAsync(nextParentId.Value, cancellationToken).ConfigureAwait(false)
                         ?? throw new InvalidOperationException($"Department '{nextParentId}' was not found.");

            ancestors.Add(parent);
            nextParentId = parent.ParentDepartmentId;
        }

        ancestors.Reverse();
        return (department, ancestors);
    }

    private static Dictionary<Guid?, List<Department>> BuildChildrenLookup(IEnumerable<Department> departments)
    {
        return departments
            .GroupBy(department => department.ParentDepartmentId)
            .ToDictionary(group => group.Key, group => group.ToList());
    }

    private static HashSet<Guid> CollectDescendantIds(
        Guid rootDepartmentId,
        IReadOnlyDictionary<Guid?, List<Department>> childrenLookup)
    {
        var descendants = new HashSet<Guid>();

        if (!childrenLookup.TryGetValue(rootDepartmentId, out var directChildren))
        {
            return descendants;
        }

        var stack = new Stack<Department>(directChildren);

        while (stack.Count > 0)
        {
            var current = stack.Pop();

            if (!descendants.Add(current.Id))
            {
                continue;
            }

            if (!childrenLookup.TryGetValue(current.Id, out var children))
            {
                continue;
            }

            foreach (var child in children)
            {
                stack.Push(child);
            }
        }

        return descendants;
    }

    private static string GetRootPathPrefix(Guid organizationId)
    {
        return $"/org/{organizationId}/dept";
    }
}
