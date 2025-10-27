using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service responsible for orchestrating employee use cases.
/// </summary>
public interface IEmployeeService
{
    /// <summary>
    ///     Retrieves all employees as DTOs suitable for API consumption.
    /// </summary>
    Task<IReadOnlyCollection<EmployeeDto>> GetAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a single employee by identifier.
    /// </summary>
    Task<EmployeeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a new employee and returns the created representation.
    /// </summary>
    Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates an existing employee and returns the updated representation.
    /// </summary>
    Task<EmployeeDto?> UpdateAsync(Guid id, UpdateEmployeeRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes an employee by identifier.
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
