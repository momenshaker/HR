using System.ComponentModel.DataAnnotations;

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

    public DateTime SubmittedAtUtc { get; init; }
}
