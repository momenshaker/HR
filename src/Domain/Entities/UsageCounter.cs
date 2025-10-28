namespace HR.Domain.Entities;

/// <summary>
///     Tracks usage of metered entitlements for a subscription.
/// </summary>
public sealed class UsageCounter
{
    public Guid Id { get; init; }

    public Guid SubscriptionId { get; init; }

    public string MetricKey { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public string MeasurementUnit { get; init; } = string.Empty;

    public decimal CurrentValue { get; init; }

    public decimal? Limit { get; init; }

    public DateOnly PeriodStart { get; init; }

    public DateOnly PeriodEnd { get; init; }

    public DateTimeOffset LastResetAtUtc { get; init; }

    public DateTimeOffset UpdatedAtUtc { get; init; }
}
