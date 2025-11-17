using System;
using System.Linq;
using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Projection helpers for subscription domain entities.
/// </summary>
public static class SubscriptionMappings
{
    public static SubscriptionDto ToDto(this Subscription subscription)
    {
        ArgumentNullException.ThrowIfNull(subscription);

        return new SubscriptionDto(
            subscription.Id,
            subscription.PlanId,
            subscription.Status.ToString(),
            subscription.Seats,
            subscription.CreatedAtUtc.UtcDateTime,
            subscription.CancelledOn?.ToDateTime(TimeOnly.MinValue),
            subscription.RenewalDate?.ToDateTime(TimeOnly.MinValue),
            subscription.CustomerId,
            subscription.OrganizationIds,
            subscription.Metadata.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.OrdinalIgnoreCase));
    }

    public static InvoiceDto ToDto(this Invoice invoice)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        return new InvoiceDto(
            invoice.Id,
            invoice.SubscriptionId ?? Guid.Empty,
            invoice.AmountDue,
            invoice.Currency,
            invoice.DueDate.ToDateTime(TimeOnly.MinValue),
            invoice.Status.ToString(),
            invoice.HostedInvoiceUrl?.ToString(),
            invoice.PdfUrl?.ToString(),
            invoice.CreatedAtUtc.UtcDateTime,
            invoice.PaidAtUtc?.UtcDateTime);
    }

    public static UsageSummaryDto ToDto(this UsageRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        return new UsageSummaryDto(
            record.SubscriptionId,
            record.FeatureKey,
            record.ConsumedUnits,
            record.UsageLimit);
    }
}
