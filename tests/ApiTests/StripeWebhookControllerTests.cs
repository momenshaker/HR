using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HR.Api.IntegrationTests;

public sealed class StripeWebhookControllerTests(WebApplicationFactory<Program> factory)
{
    private const string EndpointSecret = "whsec_test_secret";
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly WebApplicationFactory<Program> _factory = factory;

    [Fact]
    public async Task StripeWebhook_WithValidInvoicePaidEvent_InvokesInvoiceHandler()
    {
        var handler = new TestSubscriptionBillingService();
        using var client = CreateClient(handler);

        var payload = JsonSerializer.Serialize(new
        {
            id = "evt_1",
            type = "invoice.paid",
            data = new
            {
                @object = new
                {
                    id = "in_1",
                    customer = "cus_1",
                    amount_paid = 4200,
                    currency = "usd"
                }
            }
        }, SerializerOptions);

        using var request = BuildRequest(payload);
        var response = await client.SendAsync(request).ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(handler.LastInvoice);
        Assert.Equal("in_1", handler.LastInvoice!.Id);
        Assert.Equal("cus_1", handler.LastInvoice.CustomerId);
        Assert.Equal(4200, handler.LastInvoice.AmountPaid);
        Assert.Equal("usd", handler.LastInvoice.Currency);
    }

    [Fact]
    public async Task StripeWebhook_WithValidSubscriptionUpdatedEvent_InvokesSubscriptionHandler()
    {
        var handler = new TestSubscriptionBillingService();
        using var client = CreateClient(handler);

        var payload = JsonSerializer.Serialize(new
        {
            id = "evt_2",
            type = "customer.subscription.updated",
            data = new
            {
                @object = new
                {
                    id = "sub_1",
                    customer = "cus_2",
                    status = "active",
                    current_period_end = 1_700_000_000
                }
            }
        }, SerializerOptions);

        using var request = BuildRequest(payload);
        var response = await client.SendAsync(request).ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(handler.LastSubscription);
        Assert.Equal("sub_1", handler.LastSubscription!.Id);
        Assert.Equal("cus_2", handler.LastSubscription.CustomerId);
        Assert.Equal("active", handler.LastSubscription.Status);
        Assert.Equal(1_700_000_000, handler.LastSubscription.CurrentPeriodEnd);
    }

    private HttpClient CreateClient(TestSubscriptionBillingService handler)
    {
        return _factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("DOTNET_ENVIRONMENT", "Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Billing:Stripe:EndpointSecret"] = EndpointSecret,
                    ["Billing:Stripe:ToleranceInSeconds"] = "300",
                    ["Jwt:Issuer"] = "https://tests",
                    ["Jwt:Audience"] = "hr-api-tests",
                    ["Jwt:Key"] = "test-super-secret-key-1234567890",
                    ["Jwt:CustomerClaim"] = "cust",
                    ["RateLimit:RequestsPerWindow"] = "1000",
                    ["RateLimit:WindowSeconds"] = "60",
                    ["Idempotency:WindowHours"] = "24"
                });
            });
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<ISubscriptionBillingService>(handler);
            });
        }).CreateClient();
    }

    private static HttpRequestMessage BuildRequest(string payload)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/billing/webhooks/stripe")
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

        var signature = StripeTestSignatureUtility.CreateSignatureHeader(EndpointSecret, payload);
        request.Headers.Add("Stripe-Signature", signature);
        request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());
        return request;
    }

    private sealed class TestSubscriptionBillingService : ISubscriptionBillingService
    {
        public StripeInvoiceDto? LastInvoice { get; private set; }
        public StripeSubscriptionDto? LastSubscription { get; private set; }

        public Task HandleInvoicePaidAsync(StripeInvoiceDto invoice, CancellationToken cancellationToken)
        {
            LastInvoice = invoice;
            return Task.CompletedTask;
        }

        public Task HandleSubscriptionUpdatedAsync(StripeSubscriptionDto subscription, CancellationToken cancellationToken)
        {
            LastSubscription = subscription;
            return Task.CompletedTask;
        }
    }

    private static class StripeTestSignatureUtility
    {
        public static string CreateSignatureHeader(string secret, string payload, long? timestamp = null)
        {
            var ts = timestamp ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var signedPayload = $"{ts}.{payload}";
            using var hasher = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var signature = Convert.ToHexString(hasher.ComputeHash(Encoding.UTF8.GetBytes(signedPayload))).ToLowerInvariant();
            return $"t={ts},v1={signature}";
        }
    }
}

