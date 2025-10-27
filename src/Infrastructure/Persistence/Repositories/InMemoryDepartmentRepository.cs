using System.Collections.Concurrent;
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
}
