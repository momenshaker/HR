using System;
using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for employee clock-in operations.
/// </summary>
public sealed class ClockInRequest
{
    /// <summary>
    ///     Optional UTC timestamp representing when the employee clocked in. When not provided the server timestamp
    ///     will be used.
    /// </summary>
    public DateTimeOffset? TimestampUtc { get; init; }

    [MaxLength(100)]
    public string PunchType { get; init; } = "ClockIn";

    /// <summary>
    ///     Gets or sets the name of the shift the employee is clocking into.
    /// </summary>
    [MaxLength(100)]
    public string ShiftName { get; init; } = string.Empty;

    [MaxLength(100)]
    public string DeviceId { get; init; } = string.Empty;

    [MaxLength(200)]
    public string Location { get; init; } = string.Empty;

    /// <summary>
    ///     Free-form notes captured at the time of clock-in.
    /// </summary>
    [MaxLength(500)]
    public string Notes { get; init; } = string.Empty;
}
