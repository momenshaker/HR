using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

/// <summary>
///     In-memory repository implementation for attendance records.
/// </summary>
public sealed class InMemoryAttendanceRecordRepository : IAttendanceRecordRepository
{
    private readonly ConcurrentDictionary<Guid, AttendanceRecord> _records = new();

    public Task<IReadOnlyCollection<AttendanceRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<AttendanceRecord> snapshot = _records.Values.ToList();
        return Task.FromResult(snapshot);
    }

    public Task<AttendanceRecord?> GetByIdAsync(Guid attendanceId, CancellationToken cancellationToken = default)
    {
        _records.TryGetValue(attendanceId, out var record);
        return Task.FromResult(record);
    }

    public Task<AttendanceRecord> AddAsync(AttendanceRecord attendanceRecord, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(attendanceRecord);

        if (!_records.TryAdd(attendanceRecord.Id, attendanceRecord))
        {
            throw new InvalidOperationException($"An attendance record with id '{attendanceRecord.Id}' already exists.");
        }

        return Task.FromResult(attendanceRecord);
    }

    public Task<AttendanceRecord?> UpdateAsync(AttendanceRecord attendanceRecord, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(attendanceRecord);

        if (!_records.ContainsKey(attendanceRecord.Id))
        {
            return Task.FromResult<AttendanceRecord?>(null);
        }

        _records[attendanceRecord.Id] = attendanceRecord;
        return Task.FromResult<AttendanceRecord?>(attendanceRecord);
    }

    public Task<bool> RemoveAsync(Guid attendanceId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_records.TryRemove(attendanceId, out _));
    }
}
