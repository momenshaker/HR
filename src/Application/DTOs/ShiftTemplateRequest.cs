using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Payload describing a shift template definition.
/// </summary>
public sealed class ShiftTemplateRequest
{
    [Required]
    public DayOfWeek DayOfWeek { get; init; }

    [Required]
    public TimeSpan StartTime { get; init; }

    [Required]
    public TimeSpan EndTime { get; init; }

    [Range(0, 180)]
    public int BreakMinutes { get; init; }

    [Range(0, 120)]
    public int GracePeriodMinutes { get; init; }

    [Range(0, 600)]
    public int MinimumOvertimeMinutes { get; init; }
}
