using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HR.Application.Abstractions.Repositories;

namespace HR.Infrastructure.Persistence.Repositories;

/// <summary>
///     In-memory representation of employee to department assignments.
/// </summary>
public sealed class InMemoryEmployeeDepartmentRepository : IEmployeeDepartmentRepository
{
    private readonly ConcurrentDictionary<Guid, HashSet<Guid>> _employeeDepartments = new();

    public Task<IReadOnlyCollection<Guid>> GetDepartmentIdsByEmployeeAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        if (!_employeeDepartments.TryGetValue(employeeId, out var departments))
        {
            return Task.FromResult<IReadOnlyCollection<Guid>>(Array.Empty<Guid>());
        }

        lock (departments)
        {
            return Task.FromResult<IReadOnlyCollection<Guid>>(departments.ToArray());
        }
    }

    public Task AssignAsync(
        Guid employeeId,
        IReadOnlyCollection<Guid> departmentIds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(departmentIds);

        if (departmentIds.Count == 0)
        {
            return Task.CompletedTask;
        }

        var assignments = _employeeDepartments.GetOrAdd(employeeId, _ => new HashSet<Guid>());
        lock (assignments)
        {
            foreach (var departmentId in departmentIds)
            {
                assignments.Add(departmentId);
            }
        }

        return Task.CompletedTask;
    }

    public Task ReplaceAsync(
        Guid employeeId,
        IReadOnlyCollection<Guid> departmentIds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(departmentIds);

        var assignments = _employeeDepartments.GetOrAdd(employeeId, _ => new HashSet<Guid>());
        lock (assignments)
        {
            assignments.Clear();
            foreach (var departmentId in departmentIds)
            {
                assignments.Add(departmentId);
            }

            if (assignments.Count == 0)
            {
                _employeeDepartments.TryRemove(employeeId, out _);
            }
        }

        return Task.CompletedTask;
    }

    public Task UnassignAsync(
        Guid employeeId,
        IReadOnlyCollection<Guid> departmentIds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(departmentIds);

        if (!_employeeDepartments.TryGetValue(employeeId, out var assignments) || departmentIds.Count == 0)
        {
            return Task.CompletedTask;
        }

        lock (assignments)
        {
            foreach (var departmentId in departmentIds)
            {
                assignments.Remove(departmentId);
            }

            if (assignments.Count == 0)
            {
                _employeeDepartments.TryRemove(employeeId, out _);
            }
        }

        return Task.CompletedTask;
    }
}
