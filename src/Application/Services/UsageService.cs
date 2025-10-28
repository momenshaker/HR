using System.Collections.Concurrent;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using HR.Application.Mappings;
using HR.Domain.Entities;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class UsageService : IUsageService
{
    private readonly ConcurrentDictionary<(Guid SubscriptionId, string Feature), UsageRecord> _usage = new();

    public Task<UsageSummaryDto> RecordUsageAsync(Guid subscriptionId, HrFeature feature, int quantity, CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        var featureKey = feature.ToString();
        var record = _usage.AddOrUpdate(
            (subscriptionId, featureKey),
            _ => new UsageRecord(subscriptionId, featureKey, quantity, usageLimit: null),
            (_, existing) =>
            {
                existing.AddUsage(quantity);
                return existing;
            });

        return Task.FromResult(record.ToDto());
    }

    public Task<UsageSummaryDto> GetUsageAsync(Guid subscriptionId, HrFeature feature, CancellationToken cancellationToken = default)
    {
        var key = (subscriptionId, feature.ToString());
        var record = _usage.GetOrAdd(key, _ => new UsageRecord(subscriptionId, key.Item2, 0, usageLimit: null));
        return Task.FromResult(record.ToDto());
    }

    public Task ResetUsageAsync(Guid subscriptionId, HrFeature feature, CancellationToken cancellationToken = default)
    {
        var key = (subscriptionId, feature.ToString());
        _usage.TryRemove(key, out _);
        return Task.CompletedTask;
    }
}
