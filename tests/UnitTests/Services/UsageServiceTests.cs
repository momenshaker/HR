using HR.Application.Configuration;
using HR.Application.Services;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class UsageServiceTests
{
    private readonly UsageService _sut = new();

    [Fact]
    public async Task RecordUsageAsync_IncrementsConsumption()
    {
        var subscriptionId = Guid.NewGuid();

        var first = await _sut.RecordUsageAsync(subscriptionId, HrFeature.EmployeeManagement, 5, CancellationToken.None).ConfigureAwait(false);
        var second = await _sut.RecordUsageAsync(subscriptionId, HrFeature.EmployeeManagement, 3, CancellationToken.None).ConfigureAwait(false);

        Assert.Equal(5, first.ConsumedUnits);
        Assert.Equal(8, second.ConsumedUnits);
    }

    [Fact]
    public async Task ResetUsageAsync_ClearsTrackedConsumption()
    {
        var subscriptionId = Guid.NewGuid();

        await _sut.RecordUsageAsync(subscriptionId, HrFeature.LeaveManagement, 4, CancellationToken.None).ConfigureAwait(false);

        await _sut.ResetUsageAsync(subscriptionId, HrFeature.LeaveManagement, CancellationToken.None).ConfigureAwait(false);

        var snapshot = await _sut.GetUsageAsync(subscriptionId, HrFeature.LeaveManagement, CancellationToken.None).ConfigureAwait(false);

        Assert.Equal(0, snapshot.ConsumedUnits);
    }
}
