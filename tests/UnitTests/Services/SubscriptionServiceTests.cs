using HR.Application.Configuration;
using HR.Application.DTOs;
using HR.Application.Services;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class SubscriptionServiceTests
{
    private readonly SubscriptionService _sut = new();

    [Fact]
    public async Task HasEntitlementAsync_ReturnsFalse_WhenNoActiveSubscription()
    {
        var allowed = await _sut.HasEntitlementAsync(HrFeature.PayrollManagement, CancellationToken.None).ConfigureAwait(false);

        Assert.False(allowed);
    }

    [Fact]
    public async Task HasEntitlementAsync_ReturnsFalse_WhenFeatureDisabled()
    {
        var subscription = await _sut.CreateAsync(new CreateSubscriptionRequest
        {
            PlanId = Guid.NewGuid(),
            Seats = 10
        }, CancellationToken.None).ConfigureAwait(false);

        var updated = await _sut.SetEntitlementsAsync(subscription.Id, new[] { HrFeature.EmployeeManagement }, CancellationToken.None).ConfigureAwait(false);

        Assert.True(updated);

        var allowed = await _sut.HasEntitlementAsync(HrFeature.PayrollManagement, CancellationToken.None).ConfigureAwait(false);

        Assert.False(allowed);
    }

    [Fact]
    public async Task HasEntitlementAsync_ReturnsTrue_WhenFeatureEnabled()
    {
        var subscription = await _sut.CreateAsync(new CreateSubscriptionRequest
        {
            PlanId = Guid.NewGuid(),
            Seats = 5
        }, CancellationToken.None).ConfigureAwait(false);

        var updated = await _sut.SetEntitlementsAsync(subscription.Id, Enum.GetValues<HrFeature>(), CancellationToken.None).ConfigureAwait(false);

        Assert.True(updated);

        var allowed = await _sut.HasEntitlementAsync(HrFeature.PerformanceManagement, CancellationToken.None).ConfigureAwait(false);

        Assert.True(allowed);
    }
}
