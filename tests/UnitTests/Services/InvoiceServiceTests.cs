using HR.Application.Services;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class InvoiceServiceTests
{
    private readonly InvoiceService _sut = new();

    [Fact]
    public async Task CreateAsync_AssignsIdentifier()
    {
        var subscriptionId = Guid.NewGuid();

        var invoice = await _sut.CreateAsync(subscriptionId, 100m, "USD", DateTime.UtcNow.AddDays(14), CancellationToken.None).ConfigureAwait(false);

        Assert.NotEqual(Guid.Empty, invoice.Id);
        Assert.Equal(subscriptionId, invoice.SubscriptionId);
        Assert.Equal("Open", invoice.Status);
    }

    [Fact]
    public async Task GetLatestAsync_ReturnsInvoiceWithNewestDueDate()
    {
        var subscriptionId = Guid.NewGuid();

        var older = await _sut.CreateAsync(subscriptionId, 50m, "USD", DateTime.UtcNow.AddDays(7), CancellationToken.None).ConfigureAwait(false);
        var newer = await _sut.CreateAsync(subscriptionId, 75m, "USD", DateTime.UtcNow.AddDays(30), CancellationToken.None).ConfigureAwait(false);

        var latest = await _sut.GetLatestAsync(subscriptionId, CancellationToken.None).ConfigureAwait(false);

        Assert.NotNull(latest);
        Assert.Equal(newer.Id, latest!.Id);
        Assert.NotEqual(older.Id, latest.Id);
    }

    [Fact]
    public async Task MarkPaidAsync_UpdatesStatus()
    {
        var subscriptionId = Guid.NewGuid();

        var invoice = await _sut.CreateAsync(subscriptionId, 120m, "USD", DateTime.UtcNow.AddDays(10), CancellationToken.None).ConfigureAwait(false);

        var marked = await _sut.MarkPaidAsync(invoice.Id, CancellationToken.None).ConfigureAwait(false);

        Assert.True(marked);

        var latest = await _sut.GetLatestAsync(subscriptionId, CancellationToken.None).ConfigureAwait(false);
        Assert.NotNull(latest);
        Assert.Equal("Paid", latest!.Status);
        Assert.NotNull(latest.PaidAt);
    }
}
