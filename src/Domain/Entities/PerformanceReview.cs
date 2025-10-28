namespace HR.Domain.Entities;

/// <summary>
///     Represents a performance review cycle for an employee.
/// </summary>
public sealed class PerformanceReview
{
    public Guid Id { get; init; }

    public Guid EmployeeId { get; init; }

    public string CycleName { get; init; } = string.Empty;

    public DateOnly PeriodStart { get; init; }

    public DateOnly PeriodEnd { get; init; }

    public decimal OverallScore { get; init; }

    public string ManagerComments { get; init; } = string.Empty;

    public string GoalsSummary { get; init; } = string.Empty;

    public IReadOnlyCollection<PerformanceGoal> Goals { get; init; } = Array.Empty<PerformanceGoal>();

    public IReadOnlyCollection<PerformanceKpi> KeyPerformanceIndicators { get; init; } = Array.Empty<PerformanceKpi>();

    public IReadOnlyCollection<PerformanceFeedback> FeedbackCycles { get; init; } = Array.Empty<PerformanceFeedback>();

    public CompensationReview? CompensationReview { get; init; }

    public DateTime SubmittedAtUtc { get; init; }
}
