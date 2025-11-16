using System;

namespace HR.Domain.Entities;

public sealed class AttendancePunch
{
    public Guid Id { get; init; }

    public Guid AttendanceRecordId { get; init; }

    public string Type { get; init; } = string.Empty;

    public DateTimeOffset TimestampUtc { get; init; }

    public string Source { get; init; } = string.Empty;

    public string DeviceId { get; init; } = string.Empty;

    public string Location { get; init; } = string.Empty;

    public string Notes { get; init; } = string.Empty;

    public AttendanceRecord? AttendanceRecord { get; init; }
}
