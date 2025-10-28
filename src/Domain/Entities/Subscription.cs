namespace HR.Domain.Entities;

/// <summary>
///     Represents a customer's subscription to a plan.
/// </summary>
public sealed class Subscription
{
    public Guid Id { get; init; }

    public Guid CustomerId { get; init; }

    public string PlanCode { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string BillingInterval { get; init; } = string.Empty;

    public bool AutoRenew { get; init; }

    public DateOnly StartDate { get; init; }

    public DateOnly? EndDate { get; init; }

    public DateOnly? RenewalDate { get; init; }

    public DateOnly? CancelledOn { get; init; }

    public decimal Price { get; init; }

    public string Currency { get; init; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; init; }

    public DateTimeOffset? UpdatedAtUtc { get; init; }
}
