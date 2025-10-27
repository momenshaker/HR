using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository contract for managing <see cref="AttendanceRecord" /> aggregates.
/// </summary>
public interface IAttendanceRecordRepository
{
    Task<IReadOnlyCollection<AttendanceRecord>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<AttendanceRecord?> GetByIdAsync(Guid attendanceId, CancellationToken cancellationToken = default);

    Task<AttendanceRecord> AddAsync(AttendanceRecord attendanceRecord, CancellationToken cancellationToken = default);

    Task<AttendanceRecord?> UpdateAsync(AttendanceRecord attendanceRecord, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid attendanceId, CancellationToken cancellationToken = default);
}
