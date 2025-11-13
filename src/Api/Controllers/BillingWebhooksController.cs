using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using HR.Infrastructure.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace HR.Api.Controllers;

/// <summary>
///     Receives billing webhooks and dispatches to the underlying billing services.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[AllowAnonymous]
[AuditResource("BillingWebhook")]
[FeatureRequirement(HrFeature.PlatformServices)]
public sealed class BillingWebhooksController : ControllerBase
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly ISubscriptionBillingService _billingService;
    private readonly ILogger<BillingWebhooksController> _logger;
    private readonly StripeWebhookOptions _options;

    /// <summary>
    ///     Initialises a new instance of the <see cref="BillingWebhooksController"/> class.
    /// </summary>
    public BillingWebhooksController(
        ISubscriptionBillingService billingService,
        IOptionsSnapshot<StripeWebhookOptions> options,
        ILogger<BillingWebhooksController> logger)
    {
        _billingService = billingService ?? throw new ArgumentNullException(nameof(billingService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
    }

    /// <summary>
    ///     Receives and processes Stripe webhook notifications.
    /// </summary>
    [HttpPost("stripe")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ReceiveStripeWebhook(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.EndpointSecret))
        {
            _logger.LogError("Stripe webhook endpoint secret has not been configured.");
            return StatusCode(StatusCodes.Status500InternalServerError, "Stripe webhook is not configured.");
        }

        if (!Request.Headers.TryGetValue("Stripe-Signature", out var signatureHeader) || string.IsNullOrWhiteSpace(signatureHeader))
        {
            _logger.LogWarning("Stripe webhook received without signature header.");
            return BadRequest("Missing Stripe-Signature header.");
        }

        string payload;
        using (var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true))
        {
            payload = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        }

        if (string.IsNullOrWhiteSpace(payload))
        {
            _logger.LogWarning("Stripe webhook received with empty payload.");
            return BadRequest("The request payload is empty.");
        }

        var signatureValue = signatureHeader.ToString();

        if (!TryValidateSignature(signatureValue, payload, _options))
        {
            _logger.LogWarning("Stripe webhook signature validation failed.");
            return Unauthorized();
        }

        StripeWebhookEvent? webhookEvent;
        try
        {
            webhookEvent = JsonSerializer.Deserialize<StripeWebhookEvent>(payload, SerializerOptions);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Unable to deserialize Stripe webhook payload.");
            return BadRequest("Invalid payload structure.");
        }

        if (webhookEvent is null)
        {
            _logger.LogWarning("Stripe webhook payload could not be deserialized.");
            return BadRequest("Invalid payload structure.");
        }

        switch (webhookEvent.Type)
        {
            case StripeEventTypes.InvoicePaid:
                if (!TryMapInvoice(webhookEvent.Data.Object, out var invoiceDto))
                {
                    _logger.LogWarning("Stripe invoice.paid payload missing required fields.");
                    return BadRequest("Invalid invoice payload.");
                }

                await _billingService.HandleInvoicePaidAsync(invoiceDto, cancellationToken).ConfigureAwait(false);
                break;
            case StripeEventTypes.CustomerSubscriptionUpdated:
                if (!TryMapSubscription(webhookEvent.Data.Object, out var subscriptionDto))
                {
                    _logger.LogWarning("Stripe customer.subscription.updated payload missing required fields.");
                    return BadRequest("Invalid subscription payload.");
                }

                await _billingService.HandleSubscriptionUpdatedAsync(subscriptionDto, cancellationToken).ConfigureAwait(false);
                break;
            default:
                _logger.LogInformation("Stripe event type {EventType} received but not handled.", webhookEvent.Type);
                break;
        }

        if (Request.Body.CanSeek)
        {
            Request.Body.Position = 0;
        }
        return Ok();
    }

    private static bool TryMapInvoice(JsonElement json, out StripeInvoiceDto invoice)
    {
        var payload = json.Deserialize<StripeInvoicePayload>(SerializerOptions);
        if (payload is null || string.IsNullOrWhiteSpace(payload.Id))
        {
            invoice = new StripeInvoiceDto();
            return false;
        }

        invoice = new StripeInvoiceDto
        {
            Id = payload.Id!,
            CustomerId = payload.Customer,
            AmountPaid = payload.AmountPaid,
            Currency = payload.Currency
        };

        return true;
    }

    private static bool TryMapSubscription(JsonElement json, out StripeSubscriptionDto subscription)
    {
        var payload = json.Deserialize<StripeSubscriptionPayload>(SerializerOptions);
        if (payload is null || string.IsNullOrWhiteSpace(payload.Id))
        {
            subscription = new StripeSubscriptionDto();
            return false;
        }

        subscription = new StripeSubscriptionDto
        {
            Id = payload.Id!,
            CustomerId = payload.Customer,
            Status = payload.Status,
            CurrentPeriodEnd = payload.CurrentPeriodEnd
        };

        return true;
    }

    private static bool TryValidateSignature(string header, string payload, StripeWebhookOptions options)
    {
        if (!TryParseSignatureHeader(header, out var timestamp, out var signatures))
        {
            return false;
        }

        var tolerance = Math.Max(options.ToleranceInSeconds, 0);
        var signedPayload = $"{timestamp}.{payload}";
        var keyBytes = Encoding.UTF8.GetBytes(options.EndpointSecret);
        var payloadBytes = Encoding.UTF8.GetBytes(signedPayload);

        var expectedSignature = ComputeSignature(keyBytes, payloadBytes);

        var currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (tolerance > 0 && Math.Abs(currentTimestamp - timestamp) > tolerance)
        {
            return false;
        }

        foreach (var signature in signatures)
        {
            var normalizedSignature = signature.ToLowerInvariant();
            if (ConstantTimeEquals(expectedSignature, normalizedSignature))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryParseSignatureHeader(string header, out long timestamp, out IReadOnlyList<string> signatures)
    {
        timestamp = 0;
        var signatureList = new List<string>();
        var parts = header.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        string? timestampPart = null;
        foreach (var part in parts)
        {
            if (part.StartsWith("t=", StringComparison.Ordinal))
            {
                timestampPart = part[2..];
            }
            else if (part.StartsWith("v1=", StringComparison.Ordinal))
            {
                signatureList.Add(part[3..]);
            }
        }

        if (timestampPart is null || signatureList.Count == 0)
        {
            signatures = Array.Empty<string>();
            return false;
        }

        if (!long.TryParse(timestampPart, NumberStyles.Integer, CultureInfo.InvariantCulture, out timestamp))
        {
            signatures = Array.Empty<string>();
            return false;
        }

        signatures = signatureList;
        return true;
    }

    private static string ComputeSignature(byte[] key, byte[] payload)
    {
        using var hasher = new HMACSHA256(key);
        var hash = hasher.ComputeHash(payload);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static bool ConstantTimeEquals(string expected, string provided)
    {
        if (expected.Length != provided.Length)
        {
            return false;
        }

        var result = 0;
        for (var i = 0; i < expected.Length; i++)
        {
            result |= expected[i] ^ provided[i];
        }

        return result == 0;
    }

    private static class StripeEventTypes
    {
        public const string InvoicePaid = "invoice.paid";
        public const string CustomerSubscriptionUpdated = "customer.subscription.updated";
    }

    private sealed record StripeWebhookEvent(string Id, string Type, StripeWebhookEventData Data);

    private sealed record StripeWebhookEventData(JsonElement Object);

    private sealed record StripeInvoicePayload(string? Id, string? Customer, long AmountPaid, string? Currency);

    private sealed record StripeSubscriptionPayload(string? Id, string? Customer, string? Status, long? CurrentPeriodEnd);
}


