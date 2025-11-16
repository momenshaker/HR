using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service boundary for managing holiday calendars.
/// </summary>
public interface IHolidayService
{
    Task<IReadOnlyCollection<HolidayDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<HolidayDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<HolidayDto> CreateAsync(CreateHolidayRequest request, CancellationToken cancellationToken = default);

    Task<HolidayDto?> UpdateAsync(Guid id, UpdateHolidayRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
