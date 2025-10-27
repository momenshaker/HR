using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<EmployeeDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var employees = await _employeeRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return employees.Select(employee => employee.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<EmployeeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return employee?.ToDto();
    }

    /// <inheritdoc />
    public async Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = request.ToEntity();
        var createdEmployee = await _employeeRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        return createdEmployee.ToDto();
    }

    /// <inheritdoc />
    public async Task<EmployeeDto?> UpdateAsync(Guid id, UpdateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existingEmployee = await _employeeRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existingEmployee is null)
        {
            return null;
        }

        var updatedEntity = request.ApplyUpdates(existingEmployee);
        var persistedEmployee = await _employeeRepository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);

        return persistedEmployee?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _employeeRepository.RemoveAsync(id, cancellationToken);
    }
}
