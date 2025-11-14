using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service boundary for assigning schedules to employees.
/// </summary>
public interface IEmployeeScheduleService
{
    Task<IReadOnlyCollection<EmployeeScheduleDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<EmployeeScheduleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<EmployeeScheduleDto> CreateAsync(CreateEmployeeScheduleRequest request, CancellationToken cancellationToken = default);

    Task<EmployeeScheduleDto?> UpdateAsync(Guid id, UpdateEmployeeScheduleRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
