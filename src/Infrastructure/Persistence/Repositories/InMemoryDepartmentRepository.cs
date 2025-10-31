using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

/// <summary>
///     In-memory repository implementation for departments used during early development phases.
/// </summary>
public sealed class InMemoryDepartmentRepository : IDepartmentRepository
{
    private readonly ConcurrentDictionary<Guid, Department> _departments = new();

    public Task<IReadOnlyCollection<Department>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Department> snapshot = _departments.Values.ToList();
        return Task.FromResult(snapshot);
    }

    public Task<Department?> GetByIdAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        _departments.TryGetValue(departmentId, out var department);
        return Task.FromResult(department);
    }

    public Task<IReadOnlyCollection<Department>> GetByIdsAsync(
        IReadOnlyCollection<Guid> departmentIds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(departmentIds);

        if (departmentIds.Count == 0)
        {
            return Task.FromResult<IReadOnlyCollection<Department>>(Array.Empty<Department>());
        }

        var results = new List<Department>(departmentIds.Count);
        foreach (var departmentId in departmentIds)
        {
            if (_departments.TryGetValue(departmentId, out var department))
            {
                results.Add(department);
            }
        }

        return Task.FromResult<IReadOnlyCollection<Department>>(results);
    }

    public Task<Department> AddAsync(Department department, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(department);

        if (!_departments.TryAdd(department.Id, department))
        {
            throw new InvalidOperationException($"A department with id '{department.Id}' already exists.");
        }

        return Task.FromResult(department);
    }

    public Task<Department?> UpdateAsync(Department department, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(department);

        if (!_departments.ContainsKey(department.Id))
        {
            return Task.FromResult<Department?>(null);
        }

        _departments[department.Id] = department;
        return Task.FromResult<Department?>(department);
    }

    public Task<bool> RemoveAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_departments.TryRemove(departmentId, out _));
    }

    public Task<bool> ExistsByNameAsync(
        Guid organizationId,
        Guid? parentDepartmentId,
        string name,
        Guid? excludingDepartmentId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var trimmedName = name.Trim();
        var exists = _departments.Values.Any(department =>
            department.OrganizationId == organizationId &&
            (!excludingDepartmentId.HasValue || department.Id != excludingDepartmentId.Value) &&
            AreParentsEqual(department.ParentDepartmentId, parentDepartmentId) &&
            string.Equals(department.Name, trimmedName, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(exists);
    }

    public Task<bool> ExistsByCodeAsync(
        Guid organizationId,
        string code,
        Guid? excludingDepartmentId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return Task.FromResult(false);
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        var exists = _departments.Values.Any(department =>
            department.OrganizationId == organizationId &&
            (!excludingDepartmentId.HasValue || department.Id != excludingDepartmentId.Value) &&
            string.Equals(department.Code, normalizedCode, StringComparison.Ordinal));

        return Task.FromResult(exists);
    }

    private static bool AreParentsEqual(Guid? left, Guid? right)
    {
        if (!left.HasValue && !right.HasValue)
        {
            return true;
        }

        if (!left.HasValue || !right.HasValue)
        {
            return false;
        }

        return left.Value == right.Value;
    }
}
