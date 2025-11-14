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
            record.ScheduledStartTimeUtc,
            record.ScheduledEndTimeUtc,
            record.CheckInTimeUtc,
            record.CheckOutTimeUtc,
            record.ScheduledWorkMinutes,
            record.BreakMinutes,
            record.GracePeriodMinutes,
            record.TotalWorkedMinutes,
            record.LateMinutes,
            record.EarlyLeaveMinutes,
            record.OvertimeMinutes,
            record.AbsenceMinutes,
            record.Status,
            record.Source,
            record.Remarks,
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
            ScheduledStartTimeUtc = request.ScheduledStartTimeUtc?.ToUniversalTime(),
            ScheduledEndTimeUtc = request.ScheduledEndTimeUtc?.ToUniversalTime(),
            CheckInTimeUtc = request.CheckInTimeUtc?.ToUniversalTime(),
            CheckOutTimeUtc = request.CheckOutTimeUtc?.ToUniversalTime(),
            ScheduledWorkMinutes = request.ScheduledWorkMinutes,
            BreakMinutes = request.BreakMinutes,
            GracePeriodMinutes = request.GracePeriodMinutes,
            OvertimeMinutes = request.OvertimeMinutes,
            TotalWorkedMinutes = request.TotalWorkedMinutes,
            LateMinutes = request.LateMinutes,
            EarlyLeaveMinutes = request.EarlyLeaveMinutes,
            AbsenceMinutes = request.AbsenceMinutes,
            Status = request.Status.Trim(),
            Source = request.Source.Trim(),
            Remarks = request.Remarks.Trim()
        };

        foreach (var punch in request.Punches.ToDomainPunches(entity.Id))
        {
            entity.Punches.Add(punch);
        }

        return entity.ApplyDerivedMetrics();
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
            ScheduledStartTimeUtc = request.ScheduledStartTimeUtc?.ToUniversalTime(),
            ScheduledEndTimeUtc = request.ScheduledEndTimeUtc?.ToUniversalTime(),
            CheckInTimeUtc = request.CheckInTimeUtc?.ToUniversalTime(),
            CheckOutTimeUtc = request.CheckOutTimeUtc?.ToUniversalTime(),
            ScheduledWorkMinutes = request.ScheduledWorkMinutes,
            BreakMinutes = request.BreakMinutes,
            GracePeriodMinutes = request.GracePeriodMinutes,
            OvertimeMinutes = request.OvertimeMinutes,
            TotalWorkedMinutes = request.TotalWorkedMinutes,
            LateMinutes = request.LateMinutes,
            EarlyLeaveMinutes = request.EarlyLeaveMinutes,
            AbsenceMinutes = request.AbsenceMinutes,
            Status = request.Status.Trim(),
            Source = request.Source.Trim(),
            Remarks = request.Remarks.Trim()
        };

        foreach (var punch in request.Punches.ToDomainPunches(existing.Id))
        {
            entity.Punches.Add(punch);
        }

        return entity.ApplyDerivedMetrics();
    }

    private static AttendancePunchDto ToDto(this AttendancePunch punch)
    {
        ArgumentNullException.ThrowIfNull(punch);

        return new AttendancePunchDto(
            punch.Id,
            punch.Type ?? string.Empty,
            punch.TimestampUtc,
            punch.Source ?? string.Empty,
            punch.DeviceId ?? string.Empty,
            punch.Location ?? string.Empty,
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
                Source = string.IsNullOrWhiteSpace(request.Source) ? "Manual" : request.Source.Trim(),
                DeviceId = request.DeviceId.Trim(),
                Location = request.Location.Trim(),
                Notes = request.Notes.Trim()
            })
            .ToArray();
    }

    private static AttendanceRecord ApplyDerivedMetrics(this AttendanceRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        var orderedPunches = record.Punches.OrderBy(punch => punch.TimestampUtc).ToArray();

        record.CheckInTimeUtc = record.CheckInTimeUtc ?? orderedPunches.FirstOrDefault()?.TimestampUtc;
        record.CheckOutTimeUtc = record.CheckOutTimeUtc ?? orderedPunches.LastOrDefault()?.TimestampUtc;

        if (record.CheckInTimeUtc.HasValue && record.CheckOutTimeUtc.HasValue)
        {
            var duration = record.CheckOutTimeUtc.Value - record.CheckInTimeUtc.Value;
            var derivedWorkMinutes = Math.Max(0, (int)Math.Round(duration.TotalMinutes) - record.BreakMinutes);
            record.TotalWorkedMinutes = Math.Max(0, derivedWorkMinutes);
        }
        else
        {
            record.TotalWorkedMinutes = 0;
        }

        if (record.ScheduledStartTimeUtc.HasValue && record.CheckInTimeUtc.HasValue)
        {
            var graceCutoff = record.ScheduledStartTimeUtc.Value.AddMinutes(record.GracePeriodMinutes);
            record.LateMinutes = Math.Max(0, (int)Math.Round((record.CheckInTimeUtc.Value - graceCutoff).TotalMinutes));
        }

        if (record.ScheduledEndTimeUtc.HasValue && record.CheckOutTimeUtc.HasValue)
        {
            record.EarlyLeaveMinutes = Math.Max(0, (int)Math.Round((record.ScheduledEndTimeUtc.Value - record.CheckOutTimeUtc.Value).TotalMinutes));
        }

        if (record.ScheduledWorkMinutes > 0)
        {
            record.AbsenceMinutes = Math.Max(0, record.ScheduledWorkMinutes - record.TotalWorkedMinutes);
            record.OvertimeMinutes = Math.Max(0, record.TotalWorkedMinutes - record.ScheduledWorkMinutes);
        }

        if (string.IsNullOrWhiteSpace(record.Status))
        {
            record.Status = orderedPunches.Any() ? "Present" : "Absent";
        }

        if (string.IsNullOrWhiteSpace(record.Source))
        {
            record.Source = "Manual";
        }

        record.Remarks = record.Remarks?.Trim() ?? string.Empty;

        return record;
    }
}
