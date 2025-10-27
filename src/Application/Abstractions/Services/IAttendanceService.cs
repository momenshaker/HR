using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service responsible for attendance operations.
/// </summary>
public interface IAttendanceService
{
    Task<IReadOnlyCollection<AttendanceRecordDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<AttendanceRecordDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<AttendanceRecordDto> CreateAsync(CreateAttendanceRecordRequest request, CancellationToken cancellationToken = default);

    Task<AttendanceRecordDto?> UpdateAsync(Guid id, UpdateAttendanceRecordRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
