using System.Linq;
using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="AttendanceRecord" /> entities.
/// </summary>
public static class AttendanceRecordMappings
{
    public static AttendanceRecordDto ToDto(this AttendanceRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        return new AttendanceRecordDto(
            record.Id,
            record.EmployeeId,
            record.WorkDate,
            record.ShiftName,
            record.OvertimeMinutes,
            record.Status,
            record.Notes,
            record.Punches?.Select(punch => punch.ToDto()).ToArray() ?? Array.Empty<AttendancePunchDto>());
    }

    public static AttendanceRecord ToEntity(this CreateAttendanceRecordRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = new AttendanceRecord
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.EmployeeId,
            WorkDate = request.WorkDate,
            ShiftName = request.ShiftName.Trim(),
            OvertimeMinutes = request.OvertimeMinutes,
            Status = request.Status.Trim(),
            Notes = request.Notes.Trim()
        };

        foreach (var punch in request.Punches.ToDomainPunches(entity.Id))
        {
            entity.Punches.Add(punch);
        }

        return entity;
    }

    public static AttendanceRecord ApplyUpdates(this UpdateAttendanceRecordRequest request, AttendanceRecord existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        var entity = new AttendanceRecord
        {
            Id = existing.Id,
            EmployeeId = request.EmployeeId,
            WorkDate = request.WorkDate,
            ShiftName = request.ShiftName.Trim(),
            OvertimeMinutes = request.OvertimeMinutes,
            Status = request.Status.Trim(),
            Notes = request.Notes.Trim()
        };

        foreach (var punch in request.Punches.ToDomainPunches(existing.Id))
        {
            entity.Punches.Add(punch);
        }

        return entity;
    }

    private static AttendancePunchDto ToDto(this AttendancePunch punch)
    {
        ArgumentNullException.ThrowIfNull(punch);

        return new AttendancePunchDto(
            punch.Id,
            punch.Type ?? string.Empty,
            punch.TimestampUtc,
            punch.Notes ?? string.Empty);
    }

    private static IReadOnlyCollection<AttendancePunch> ToDomainPunches(
        this IEnumerable<AttendancePunchRequest>? requests,
        Guid attendanceRecordId = default)
    {
        if (requests is null)
        {
            return Array.Empty<AttendancePunch>();
        }

        return requests
            .Select(request => new AttendancePunch
            {
                Id = request.Id.GetValueOrDefault() == Guid.Empty ? Guid.NewGuid() : request.Id.GetValueOrDefault(),
                AttendanceRecordId = attendanceRecordId,
                Type = request.Type.Trim(),
                TimestampUtc = request.TimestampUtc.ToUniversalTime(),
                Notes = request.Notes.Trim()
            })
            .ToArray();
    }
}
