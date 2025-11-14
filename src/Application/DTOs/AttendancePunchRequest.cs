using System;
using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Payload describing a single attendance punch (check-in, break, check-out, etc.).
/// </summary>
public sealed class AttendancePunchRequest
{
    public Guid? Id { get; init; }

    [Required]
    [MaxLength(100)]
    public string Type { get; init; } = string.Empty;

    [Required]
    public DateTimeOffset TimestampUtc { get; init; }

    [MaxLength(100)]
    public string Source { get; init; } = string.Empty;

    [MaxLength(100)]
    public string DeviceId { get; init; } = string.Empty;

    [MaxLength(200)]
    public string Location { get; init; } = string.Empty;

    [MaxLength(500)]
    public string Notes { get; init; } = string.Empty;
}
