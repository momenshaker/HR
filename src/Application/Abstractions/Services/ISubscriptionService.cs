using HR.Application.Configuration;
using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service abstraction for managing tenant subscriptions.
/// </summary>
public interface ISubscriptionService
{
    Task<IReadOnlyCollection<SubscriptionDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<SubscriptionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<SubscriptionDto> CreateAsync(CreateSubscriptionRequest request, CancellationToken cancellationToken = default);

    Task<SubscriptionDto?> UpdateAsync(Guid id, UpdateSubscriptionRequest request, CancellationToken cancellationToken = default);

    Task<bool> CancelAsync(Guid id, CancellationToken cancellationToken = default);

    Task<SubscriptionDto?> GetActiveSubscriptionAsync(CancellationToken cancellationToken = default);

    Task<bool> HasEntitlementAsync(HrFeature feature, CancellationToken cancellationToken = default);

    Task<bool> SetEntitlementsAsync(Guid subscriptionId, IEnumerable<HrFeature> features, CancellationToken cancellationToken = default);
}
