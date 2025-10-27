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
}
