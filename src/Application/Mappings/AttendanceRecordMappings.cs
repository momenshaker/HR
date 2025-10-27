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
            record.ClockInUtc,
            record.ClockOutUtc,
            record.OvertimeMinutes,
            record.Status,
            record.Notes);
    }

    public static AttendanceRecord ToEntity(this CreateAttendanceRecordRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new AttendanceRecord
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.EmployeeId,
            WorkDate = request.WorkDate,
            ShiftName = request.ShiftName.Trim(),
            ClockInUtc = request.ClockInUtc,
            ClockOutUtc = request.ClockOutUtc,
            OvertimeMinutes = request.OvertimeMinutes,
            Status = request.Status.Trim(),
            Notes = request.Notes.Trim()
        };
    }

    public static AttendanceRecord ApplyUpdates(this UpdateAttendanceRecordRequest request, AttendanceRecord existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        return new AttendanceRecord
        {
            Id = existing.Id,
            EmployeeId = request.EmployeeId,
            WorkDate = request.WorkDate,
            ShiftName = request.ShiftName.Trim(),
            ClockInUtc = request.ClockInUtc,
            ClockOutUtc = request.ClockOutUtc,
            OvertimeMinutes = request.OvertimeMinutes,
            Status = request.Status.Trim(),
            Notes = request.Notes.Trim()
        };
    }
}
