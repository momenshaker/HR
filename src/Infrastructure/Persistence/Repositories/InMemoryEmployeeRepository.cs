using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

/// <summary>
///     Temporary in-memory repository implementation for early development and testing.
/// </summary>
public sealed class InMemoryEmployeeRepository : IEmployeeRepository
{
    private readonly ConcurrentDictionary<Guid, Employee> _employees = new();

    public Task<IReadOnlyCollection<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Employee> snapshot = _employees.Values.ToList();
        return Task.FromResult(snapshot);
    }

    public Task<Employee?> GetByIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        _employees.TryGetValue(employeeId, out var employee);
        return Task.FromResult(employee);
    }

    public Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(employee);

        if (!_employees.TryAdd(employee.Id, employee))
        {
            throw new InvalidOperationException($"An employee with id '{employee.Id}' already exists.");
        }

        return Task.FromResult(employee);
    }
}
