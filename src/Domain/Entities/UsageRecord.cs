namespace HR.Domain.Entities;

/// <summary>
///     Represents usage consumption for a specific subscription feature entitlement.
/// </summary>
public sealed class UsageRecord
{
    public UsageRecord(Guid subscriptionId, string featureKey, int consumedUnits, int? usageLimit)
    {
        SubscriptionId = subscriptionId;
        FeatureKey = featureKey;
        ConsumedUnits = consumedUnits;
        UsageLimit = usageLimit;
    }

    public Guid SubscriptionId { get; }

    public string FeatureKey { get; }

    public int ConsumedUnits { get; private set; }

    public int? UsageLimit { get; }

    public void AddUsage(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        ConsumedUnits += quantity;
    }

    public void Reset()
    {
        ConsumedUnits = 0;
    }
}
