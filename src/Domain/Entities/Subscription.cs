using System.Collections.Concurrent;

namespace HR.Domain.Entities;

/// <summary>
/// Represents a customer's subscription to a plan, including billing metadata,
/// feature entitlements, lifecycle status, pricing, and renewal behavior.
/// </summary>
public sealed class Subscription
{
    private readonly ConcurrentDictionary<string, string> _metadata;
    private IReadOnlyCollection<string> _entitledFeatures;

    // EF Core parameterless constructor
    private Subscription()
    {
        _metadata = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        _entitledFeatures = Array.Empty<string>();
    }

    public Subscription(
        Guid id,
        Guid customerId,
        Guid planId,
        string planCode,
        SubscriptionStatus status,
        SubscriptionInterval billingInterval,
        bool autoRenew,
        int seats,
        DateOnly startDate,
        DateOnly? endDate,
        DateOnly? renewalDate,
        DateOnly? cancelledOn,
        decimal price,
        string currency,
        DateTimeOffset createdAtUtc,
        IEnumerable<KeyValuePair<string, string>>? metadata = null,
        IEnumerable<string>? entitledFeatures = null)
        : this()
    {
        Id = id;
        CustomerId = customerId;
        PlanId = planId;
        PlanCode = planCode ?? string.Empty;
        Status = status;
        BillingInterval = billingInterval;
        AutoRenew = autoRenew;
        Seats = seats;
        StartDate = startDate;
        EndDate = endDate;
        RenewalDate = renewalDate;
        CancelledOn = cancelledOn;
        Price = price;
        Currency = currency ?? string.Empty;
        CreatedAtUtc = createdAtUtc;

        if (metadata is not null)
        {
            foreach (var pair in metadata)
            {
                _metadata[pair.Key] = pair.Value;
            }
        }

        _entitledFeatures = (entitledFeatures ?? Array.Empty<string>())
            .Where(f => !string.IsNullOrWhiteSpace(f))
            .Select(f => f.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public Guid Id { get; }

    public Guid CustomerId { get; private set; }

    public Guid PlanId { get; private set; }

    public string PlanCode { get; private set; } = string.Empty;

    public SubscriptionStatus Status { get; private set; }

    public SubscriptionInterval BillingInterval { get; private set; }

    public bool AutoRenew { get; private set; }

    public int Seats { get; private set; }

    public DateOnly StartDate { get; private set; }

    public DateOnly? EndDate { get; private set; }

    /// <summary>Next planned renewal date (business date, no time component).</summary>
    public DateOnly? RenewalDate { get; private set; }

    /// <summary>Business date the subscription was cancelled on, if cancelled.</summary>
    public DateOnly? CancelledOn { get; private set; }

    /// <summary>Current price per billing interval (plan price at time of subscription).</summary>
    public decimal Price { get; private set; }

    /// <summary>ISO 4217 currency code (e.g., "USD").</summary>
    public string Currency { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; }

    public DateTimeOffset? UpdatedAtUtc { get; private set; }

    public IReadOnlyDictionary<string, string> Metadata => _metadata;

    public IReadOnlyCollection<string> EntitledFeatures => _entitledFeatures;

    // ---------------------------
    // Domain behavior / mutators
    // ---------------------------

    public void UpdatePlan(Guid planId, string? planCode = null)
    {
        PlanId = planId;
        if (planCode is not null) PlanCode = planCode;
        Touch();
    }

    public void UpdateSeats(int seats)
    {
        Seats = seats;
        Touch();
    }

    public void UpdateStatus(SubscriptionStatus status)
    {
        Status = status;
        Touch();
    }

    public void UpdateBillingInterval(SubscriptionInterval interval)
    {
        BillingInterval = interval;
        Touch();
    }

    public void ToggleAutoRenew(bool enabled)
    {
        AutoRenew = enabled;
        Touch();
    }

    public void UpdateRenewalDate(DateOnly? renewalDate)
    {
        RenewalDate = renewalDate;
        Touch();
    }

    public void SetEndDate(DateOnly? endDate)
    {
        EndDate = endDate;
        Touch();
    }

    public void Cancel(DateOnly cancelledOn)
    {
        Status = SubscriptionStatus.Canceled;
        CancelledOn = cancelledOn;
        AutoRenew = false;
        Touch();
    }

    public void Resume()
    {
        Status = SubscriptionStatus.Active;
        CancelledOn = null;
        Touch();
    }

    public void UpdatePrice(decimal price, string currency)
    {
        Price = price;
        Currency = currency ?? string.Empty;
        Touch();
    }

    public void LinkCustomer(Guid customerId)
    {
        CustomerId = customerId;
        Touch();
    }

    public void SetMetadata(IEnumerable<KeyValuePair<string, string>> metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        _metadata.Clear();
        foreach (var pair in metadata)
        {
            _metadata[pair.Key] = pair.Value;
        }
        Touch();
    }

    public void SetEntitledFeatures(IEnumerable<string> features)
    {
        ArgumentNullException.ThrowIfNull(features);

        _entitledFeatures = features
            .Where(feature => !string.IsNullOrWhiteSpace(feature))
            .Select(feature => feature.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        Touch();
    }

    public bool HasEntitlement(string feature)
    {
        if (Status != SubscriptionStatus.Active) return false;
        return _entitledFeatures.Contains(feature, StringComparer.OrdinalIgnoreCase);
    }

    // ---------------------------
    // Helpers
    // ---------------------------

    private void Touch() => UpdatedAtUtc = DateTimeOffset.UtcNow;
}

/// <summary>
/// Billing interval for a subscription plan.
/// </summary>
public enum SubscriptionInterval
{
    Monthly = 1,
    Quarterly = 2,
    SemiAnnual = 3,
    Annual = 4
}
