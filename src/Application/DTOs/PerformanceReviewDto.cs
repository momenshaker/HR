namespace HR.Application.DTOs;

/// <summary>
///     Read model representing a performance review.
/// </summary>
public sealed record PerformanceReviewDto(
    Guid Id,
    Guid EmployeeId,
    string CycleName,
    DateOnly PeriodStart,
    DateOnly PeriodEnd,
    decimal OverallScore,
    string ManagerComments,
    string GoalsSummary,
    IReadOnlyCollection<PerformanceGoalDto> Goals,
    IReadOnlyCollection<PerformanceKpiDto> KeyPerformanceIndicators,
    IReadOnlyCollection<PerformanceFeedbackDto> FeedbackCycles,
    CompensationReviewDto? CompensationReview,
    DateTime SubmittedAtUtc);
