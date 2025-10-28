namespace HR.Domain.Entities;

/// <summary>
///     Represents a feature or quota entitlement granted by a subscription or plan.
/// </summary>
public sealed class SubscriptionEntitlement
{
    public Guid Id { get; init; }

    public Guid? SubscriptionId { get; init; }

    public string PlanCode { get; init; } = string.Empty;

    public string FeatureKey { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string MeasurementUnit { get; init; } = string.Empty;

    public int? Quantity { get; init; }

    public bool IsEnabled { get; init; }

    public DateOnly? EffectiveFrom { get; init; }

    public DateOnly? EffectiveTo { get; init; }

    public DateTimeOffset CreatedAtUtc { get; init; }
}
