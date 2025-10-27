using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for creating a performance review.
/// </summary>
public sealed class CreatePerformanceReviewRequest
{
    [Required]
    public Guid EmployeeId { get; init; }

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
}
