using System.ComponentModel.DataAnnotations;
using HR.Application.Common.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for updating a performance review.
/// </summary>
public sealed class UpdatePerformanceReviewRequest
{
    [Required]
    [MaxLength(100)]
    public string CycleName { get; init; } = string.Empty;

    [Required]
    public DateOnly PeriodStart { get; init; }

    [Required]
    public DateOnly PeriodEnd { get; init; }

    [Range(0, 5)]
    public decimal OverallScore { get; init; }

    [MaxLength(2000)]
    public string ManagerComments { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string GoalsSummary { get; init; } = string.Empty;

    [ValidateComplexType]
    public IReadOnlyCollection<PerformanceGoalRequest> Goals { get; init; } = Array.Empty<PerformanceGoalRequest>();

    [ValidateComplexType]
    public IReadOnlyCollection<PerformanceKpiRequest> KeyPerformanceIndicators { get; init; } = Array.Empty<PerformanceKpiRequest>();

    [ValidateComplexType]
    public IReadOnlyCollection<PerformanceFeedbackRequest> FeedbackCycles { get; init; } = Array.Empty<PerformanceFeedbackRequest>();

    [ValidateComplexType]
    public CompensationReviewRequest? CompensationReview { get; init; }

    public DateTime SubmittedAtUtc { get; init; }
}
