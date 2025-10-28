using System.Text.Json;
using HR.Api.Contracts;
using HR.Api.Middleware;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace HR.UnitTests.Middleware;

public sealed class SubscriptionGuardMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ReturnsForbidden_WhenEntitlementMissing()
    {
        var subscriptionService = new Mock<ISubscriptionService>();
        subscriptionService
            .Setup(service => service.HasEntitlementAsync(HrFeature.PayrollManagement, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var endpoint = new Endpoint(
            _ => Task.CompletedTask,
            new EndpointMetadataCollection(new RequiresSubscriptionEntitlementAttribute(HrFeature.PayrollManagement)),
            "test");

        context.Features.Set<IEndpointFeature>(new EndpointFeature { Endpoint = endpoint });

        var middleware = new SubscriptionGuardMiddleware(_ => Task.CompletedTask, NullLogger<SubscriptionGuardMiddleware>.Instance);

        await middleware.InvokeAsync(context, subscriptionService.Object).ConfigureAwait(false);

        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var payload = await JsonSerializer.DeserializeAsync<ErrorResponse>(
            context.Response.Body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).ConfigureAwait(false);

        Assert.NotNull(payload);
        Assert.Equal("subscription_entitlement_denied", payload!.Code);
    }

    [Fact]
    public async Task InvokeAsync_ContinuesPipeline_WhenEntitlementPresent()
    {
        var subscriptionService = new Mock<ISubscriptionService>();
        subscriptionService
            .Setup(service => service.HasEntitlementAsync(HrFeature.PayrollManagement, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var context = new DefaultHttpContext();
        var endpoint = new Endpoint(
            _ => Task.CompletedTask,
            new EndpointMetadataCollection(new RequiresSubscriptionEntitlementAttribute(HrFeature.PayrollManagement)),
            "test");

        context.Features.Set<IEndpointFeature>(new EndpointFeature { Endpoint = endpoint });

        var nextInvoked = false;
        RequestDelegate next = _ =>
        {
            nextInvoked = true;
            return Task.CompletedTask;
        };

        var middleware = new SubscriptionGuardMiddleware(next, NullLogger<SubscriptionGuardMiddleware>.Instance);

        await middleware.InvokeAsync(context, subscriptionService.Object).ConfigureAwait(false);

        Assert.True(nextInvoked);
        Assert.NotEqual(StatusCodes.Status403Forbidden, context.Response.StatusCode);
    }

    private sealed class EndpointFeature : IEndpointFeature
    {
        public Endpoint? Endpoint { get; set; }
    }
}
