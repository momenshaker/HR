namespace HR.Application.DTOs;

public sealed record LeavePreviewDto(
    decimal DurationDays,
    decimal CurrentAvailable,
    decimal Reserved,
    decimal AvailableAfter);

