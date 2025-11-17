using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using HR.Application.Mappings;
using HR.Domain.Entities;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class SubscriptionService : ISubscriptionService
{
    private readonly ConcurrentDictionary<Guid, Subscription> _subscriptions = new();

    public SubscriptionService()
    {
        SeedDefaultSubscription(_subscriptions);
    }

    public Task<IReadOnlyCollection<SubscriptionDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<SubscriptionDto> snapshot = _subscriptions.Values
            .Select(subscription => subscription.ToDto())
            .ToArray();

        return Task.FromResult(snapshot);
    }

    public Task<SubscriptionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _subscriptions.TryGetValue(id, out var subscription);
        return Task.FromResult(subscription?.ToDto());
    }

    public Task<SubscriptionDto> CreateAsync(CreateSubscriptionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var createdAt = DateTimeOffset.UtcNow;
        var startDate = DateOnly.FromDateTime(createdAt.UtcDateTime);
        var renewalDate = request.TrialPeriodDays.HasValue
            ? startDate.AddDays(request.TrialPeriodDays.Value)
            : startDate.AddMonths(1);

        var entitlementKeys = Enum.GetValues<HrFeature>()
            .Where(feature => feature != HrFeature.PlatformServices)
            .Select(feature => feature.ToString());

        var subscription = new Subscription(
            id: Guid.NewGuid(),
            customerId: Guid.Empty,
            planId: request.PlanId,
            planCode: string.Empty,
            status: SubscriptionStatus.Active,
            billingInterval: SubscriptionInterval.Monthly,
            autoRenew: true,
            seats: request.Seats,
            startDate: startDate,
            endDate: null,
            renewalDate: renewalDate,
            cancelledOn: null,
            price: 0m,
            currency: string.Empty,
            createdAtUtc: createdAt,
            metadata: request.Metadata,
            entitledFeatures: entitlementKeys);

        if (!_subscriptions.TryAdd(subscription.Id, subscription))
        {
            throw new InvalidOperationException($"Subscription with id '{subscription.Id}' already exists.");
        }

        return Task.FromResult(subscription.ToDto());
    }

    public Task<SubscriptionDto?> UpdateAsync(Guid id, UpdateSubscriptionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!_subscriptions.TryGetValue(id, out var subscription))
        {
            return Task.FromResult<SubscriptionDto?>(null);
        }

        if (request.PlanId.HasValue)
        {
            subscription.UpdatePlan(request.PlanId.Value);
        }

        if (request.Seats.HasValue)
        {
            subscription.UpdateSeats(request.Seats.Value);
        }

        if (request.Metadata is not null)
        {
            subscription.SetMetadata(request.Metadata);
        }

        if (request.RenewsAt.HasValue)
        {
            var renewal = DateOnly.FromDateTime(request.RenewsAt.Value.Date);
            subscription.UpdateRenewalDate(renewal);
        }

        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<SubscriptionStatus>(request.Status, true, out var status))
        {
            subscription.UpdateStatus(status);
        }

        return Task.FromResult<SubscriptionDto?>(subscription.ToDto());
    }

    public Task<bool> CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!_subscriptions.TryGetValue(id, out var subscription))
        {
            return Task.FromResult(false);
        }

        subscription.Cancel(DateOnly.FromDateTime(DateTime.UtcNow));
        return Task.FromResult(true);
    }

    public Task<SubscriptionDto?> GetActiveSubscriptionAsync(CancellationToken cancellationToken = default)
    {
        var subscription = _subscriptions.Values
            .OrderByDescending(item => item.CreatedAtUtc)
            .FirstOrDefault(item => item.Status == SubscriptionStatus.Active);

        return Task.FromResult(subscription?.ToDto());
    }

    public Task<bool> HasEntitlementAsync(HrFeature feature, CancellationToken cancellationToken = default)
    {
        var subscription = _subscriptions.Values.FirstOrDefault(item => item.Status == SubscriptionStatus.Active);
        if (subscription is null)
        {
            return Task.FromResult(false);
        }

        var entitlementKey = feature.ToString();
        var allowed = subscription.HasEntitlement(entitlementKey);
        return Task.FromResult(allowed);
    }

    public Task<bool> SetEntitlementsAsync(Guid subscriptionId, IEnumerable<HrFeature> features, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(features);

        if (!_subscriptions.TryGetValue(subscriptionId, out var subscription))
        {
            return Task.FromResult(false);
        }

        var featureKeys = features.Select(feature => feature.ToString());
        subscription.SetEntitledFeatures(featureKeys);
        return Task.FromResult(true);
    }

    public Task<bool> SetOrganizationsAsync(Guid subscriptionId, IEnumerable<Guid> organizationIds, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(organizationIds);
        if (!_subscriptions.TryGetValue(subscriptionId, out var subscription))
        {
            return Task.FromResult(false);
        }

        subscription.SetOrganizations(organizationIds);
        return Task.FromResult(true);
    }

    private static void SeedDefaultSubscription(ConcurrentDictionary<Guid, Subscription> store)
    {
        if (store.Count > 0)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var startDate = DateOnly.FromDateTime(now.UtcDateTime);
        var renewalDate = startDate.AddMonths(1);
        var entitlementKeys = Enum.GetValues<HrFeature>()
            .Where(feature => feature != HrFeature.PlatformServices)
            .Select(feature => feature.ToString())
            .ToArray();

        var subscription = new Subscription(
            id: Guid.NewGuid(),
            customerId: Guid.Empty,
            planId: Guid.Empty,
            planCode: "starter",
            status: SubscriptionStatus.Active,
            billingInterval: SubscriptionInterval.Monthly,
            autoRenew: true,
            seats: 100,
            startDate: startDate,
            endDate: null,
            renewalDate: renewalDate,
            cancelledOn: null,
            price: 0m,
            currency: "USD",
            createdAtUtc: now,
            metadata: Array.Empty<KeyValuePair<string, string>>(),
            entitledFeatures: entitlementKeys);

        store.TryAdd(subscription.Id, subscription);
    }
}
