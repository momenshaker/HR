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
    public DateTime? TimestampUtc { get; init; }

    /// <summary>
    ///     Free-form notes captured at the time of clock-out.
    /// </summary>
    [MaxLength(500)]
    public string Notes { get; init; } = string.Empty;
}
