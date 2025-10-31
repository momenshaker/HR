using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository contract for managing <see cref="Employee" /> aggregates.
/// </summary>
public interface IEmployeeRepository
{
    /// <summary>
    ///     Retrieves all employees in the system.
    /// </summary>
    Task<IReadOnlyCollection<Employee>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a specific employee by their identifier.
    /// </summary>
    Task<Employee?> GetByIdAsync(Guid employeeId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Persists a new employee entity.
    /// </summary>
    Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates an existing employee entity.
    /// </summary>
    Task<Employee?> UpdateAsync(Employee employee, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Removes an employee from the system.
    /// </summary>
    Task<bool> RemoveAsync(Guid employeeId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Determines whether an employee with the specified email address already exists.
    /// </summary>
    Task<bool> ExistsByEmailAsync(
        string email,
        Guid? excludingEmployeeId = null,
        CancellationToken cancellationToken = default);
}
