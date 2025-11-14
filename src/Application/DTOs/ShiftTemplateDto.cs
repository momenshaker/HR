namespace HR.Application.DTOs;

/// <summary>
///     Read model representing a single shift template.
/// </summary>
public sealed record ShiftTemplateDto(
    Guid Id,
    DayOfWeek DayOfWeek,
    TimeSpan StartTime,
    TimeSpan EndTime,
    int BreakMinutes,
    int GracePeriodMinutes,
    int MinimumOvertimeMinutes);
