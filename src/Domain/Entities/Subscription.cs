using System.Collections.Concurrent;

namespace HR.Domain.Entities;

/// <summary>
///     Represents a tenant subscription including billing metadata and feature entitlements.
/// </summary>
public sealed class Subscription
{
    private readonly ConcurrentDictionary<string, string> _metadata;
    private IReadOnlyCollection<string> _entitledFeatures;

    public Subscription(
        Guid id,
        Guid planId,
        SubscriptionStatus status,
        int seats,
        DateTime createdAtUtc,
        DateTime? canceledAtUtc,
        DateTime? renewsAtUtc,
        IEnumerable<KeyValuePair<string, string>>? metadata,
        IEnumerable<string>? entitledFeatures)
    {
        Id = id;
        PlanId = planId;
        Status = status;
        Seats = seats;
        CreatedAtUtc = createdAtUtc;
        CanceledAtUtc = canceledAtUtc;
        RenewsAtUtc = renewsAtUtc;

        _metadata = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (metadata is not null)
        {
            foreach (var pair in metadata)
            {
                _metadata[pair.Key] = pair.Value;
            }
        }

        _entitledFeatures = (entitledFeatures ?? Array.Empty<string>())
            .Select(feature => feature)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public Guid Id { get; }

    public Guid PlanId { get; private set; }

    public SubscriptionStatus Status { get; private set; }

    public int Seats { get; private set; }

    public DateTime CreatedAtUtc { get; }

    public DateTime? CanceledAtUtc { get; private set; }

    public DateTime? RenewsAtUtc { get; private set; }

    public IReadOnlyDictionary<string, string> Metadata => _metadata;

    public IReadOnlyCollection<string> EntitledFeatures => _entitledFeatures;

    public void UpdatePlan(Guid planId)
    {
        PlanId = planId;
    }

    public void UpdateSeats(int seats)
    {
        Seats = seats;
    }

    public void UpdateStatus(SubscriptionStatus status)
    {
        Status = status;
    }

    public void UpdateRenewsAt(DateTime? renewsAtUtc)
    {
        RenewsAtUtc = renewsAtUtc;
    }

    public void Cancel(DateTime canceledAtUtc)
    {
        Status = SubscriptionStatus.Canceled;
        CanceledAtUtc = canceledAtUtc;
    }

    public void SetMetadata(IEnumerable<KeyValuePair<string, string>> metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);

        _metadata.Clear();
        foreach (var pair in metadata)
        {
            _metadata[pair.Key] = pair.Value;
        }
    }

    public void SetEntitledFeatures(IEnumerable<string> features)
    {
        ArgumentNullException.ThrowIfNull(features);

        _entitledFeatures = features
            .Where(feature => !string.IsNullOrWhiteSpace(feature))
            .Select(feature => feature.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public bool HasEntitlement(string feature)
    {
        if (Status != SubscriptionStatus.Active)
        {
            return false;
        }

        return _entitledFeatures.Contains(feature, StringComparer.OrdinalIgnoreCase);
    }
}
