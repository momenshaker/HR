using System;
using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for employee clock-out operations.
/// </summary>
public sealed class ClockOutRequest
{
    /// <summary>
    ///     Optional UTC timestamp representing when the employee clocked out. When not provided the server timestamp
    ///     will be used.
    /// </summary>
    public DateTimeOffset? TimestampUtc { get; init; }

    [MaxLength(100)]
    public string PunchType { get; init; } = "ClockOut";

    [MaxLength(100)]
    public string DeviceId { get; init; } = string.Empty;

    [MaxLength(200)]
    public string Location { get; init; } = string.Empty;

    /// <summary>
    ///     Free-form notes captured at the time of clock-out.
    /// </summary>
    [MaxLength(500)]
    public string Notes { get; init; } = string.Empty;
}
