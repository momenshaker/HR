using System;

namespace HR.Application.DTOs;

/// <summary>
///     Read model representing a timestamped attendance event.
/// </summary>
public sealed record AttendancePunchDto(
    Guid Id,
    string Type,
    DateTimeOffset TimestampUtc,
    string Source,
    string DeviceId,
    string Location,
    string Notes);
