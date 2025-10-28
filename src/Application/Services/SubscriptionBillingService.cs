using System;
using System.Threading;
using System.Threading.Tasks;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace HR.Application.Services;

/// <summary>
///     Default implementation of <see cref="ISubscriptionBillingService"/> that records webhook events for auditing.
/// </summary>
public sealed class SubscriptionBillingService(ILogger<SubscriptionBillingService> logger) : ISubscriptionBillingService
{
    private readonly ILogger<SubscriptionBillingService> _logger = logger;

    /// <inheritdoc />
    public Task HandleInvoicePaidAsync(StripeInvoiceDto invoice, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        _logger.LogInformation(
            "Stripe invoice paid: {InvoiceId} for customer {CustomerId} with amount {AmountPaid} {Currency}",
            invoice.Id,
            invoice.CustomerId,
            invoice.AmountPaid,
            invoice.Currency);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task HandleSubscriptionUpdatedAsync(StripeSubscriptionDto subscription, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(subscription);

        _logger.LogInformation(
            "Stripe subscription updated: {SubscriptionId} for customer {CustomerId} is now {Status} with period end {PeriodEnd}",
            subscription.Id,
            subscription.CustomerId,
            subscription.Status,
            subscription.CurrentPeriodEnd);

        return Task.CompletedTask;
    }
}

