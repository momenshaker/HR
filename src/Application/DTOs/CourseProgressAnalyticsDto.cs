using System;

namespace HR.Application.DTOs;

/// <summary>
///     Aggregated metrics describing course progress trends.
/// </summary>
public sealed record CourseProgressAnalyticsDto(
    Guid CourseId,
    int TotalEnrollments,
    int ActiveEnrollments,
    int CompletedEnrollments,
    decimal AverageCompletionPercentage,
    DateTime GeneratedOnUtc);
