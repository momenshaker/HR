using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using HR.Api.Contracts;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HR.Api.IntegrationTests;

public sealed class SubscriptionGuardIntegrationTests(CustomWebApplicationFactory factory) : IClassFixture<SubscriptionGuardIntegrationTests.CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task PayrollEndpoint_ReturnsForbidden_WhenEntitlementMissing()
    {
        var subscription = await EnsureActiveSubscriptionAsync().ConfigureAwait(false);

        await UpdateEntitlementsAsync(subscription.Id, new[] { HrFeature.EmployeeManagement }).ConfigureAwait(false);

        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/v1/PayrollRuns");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            JwtTestHelper.CreateToken("https://tests", "hr-api-tests", "test-super-secret-key-1234567890", new[] { "Admin" })
        );

        var response = await _client.SendAsync(requestMessage).ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>().ConfigureAwait(false);
        Assert.NotNull(error);
        Assert.Equal("subscription_entitlement_denied", error!.Code);
    }

    [Fact]
    public async Task PerformanceEndpoint_AllowsAccess_WhenEntitlementGranted()
    {
        var subscription = await EnsureActiveSubscriptionAsync().ConfigureAwait(false);

        await UpdateEntitlementsAsync(subscription.Id, Enum.GetValues<HrFeature>()).ConfigureAwait(false);

        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/v1/PerformanceReviews");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            JwtTestHelper.CreateToken("https://tests", "hr-api-tests", "test-super-secret-key-1234567890", new[] { "Admin" })
        );

        var response = await _client.SendAsync(requestMessage).ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<SubscriptionDto> EnsureActiveSubscriptionAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();

        var existing = await subscriptionService.GetActiveSubscriptionAsync().ConfigureAwait(false);
        if (existing is not null)
        {
            return existing;
        }

        return await subscriptionService.CreateAsync(new CreateSubscriptionRequest
        {
            PlanId = Guid.NewGuid(),
            Seats = 25
        }, CancellationToken.None).ConfigureAwait(false);
    }

    private async Task UpdateEntitlementsAsync(Guid subscriptionId, IEnumerable<HrFeature> features)
    {
        using var scope = _factory.Services.CreateScope();
        var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();
        await subscriptionService.SetEntitlementsAsync(subscriptionId, features, CancellationToken.None).ConfigureAwait(false);
    }

    public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSetting("DOTNET_ENVIRONMENT", "Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["HrPlatform:Data:RepositoryProvider"] = "InMemory",
                    ["Jwt:Issuer"] = "https://tests",
                    ["Jwt:Audience"] = "hr-api-tests",
                    ["Jwt:Key"] = "test-super-secret-key-1234567890",
                    ["Jwt:CustomerClaim"] = "cust",
                    ["RateLimit:RequestsPerWindow"] = "1000",
                    ["RateLimit:WindowSeconds"] = "60",
                    ["Idempotency:WindowHours"] = "24"
                });
            });
        }
    }
}
