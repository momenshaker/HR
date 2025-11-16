using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service boundary for managing work schedules and shift templates.
/// </summary>
public interface IWorkScheduleService
{
    Task<IReadOnlyCollection<WorkScheduleDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<WorkScheduleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<WorkScheduleDto> CreateAsync(CreateWorkScheduleRequest request, CancellationToken cancellationToken = default);

    Task<WorkScheduleDto?> UpdateAsync(Guid id, UpdateWorkScheduleRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
