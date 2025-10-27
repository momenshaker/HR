namespace HR.Domain.Entities;

/// <summary>
///     Represents a snapshot of HR analytics metrics captured at a point in time.
/// </summary>
public sealed class AnalyticsSnapshot
{
    public Guid Id { get; init; }

    public DateTime CapturedAtUtc { get; init; }

    public int Headcount { get; init; }

    public decimal TurnoverRate { get; init; }

    public decimal AverageTenureMonths { get; init; }

    public decimal HiringVelocity { get; init; }

    public decimal EngagementScore { get; init; }

    public string Commentary { get; init; } = string.Empty;
}
