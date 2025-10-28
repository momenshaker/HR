using System.Threading;
using System.Threading.Tasks;
using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Handles subscription billing lifecycle events emitted by external payment processors.
/// </summary>
public interface ISubscriptionBillingService
{
    /// <summary>
    ///     Processes a paid invoice notification.
    /// </summary>
    /// <param name="invoice">The invoice payload extracted from the webhook.</param>
    /// <param name="cancellationToken">Token used to signal cancellation.</param>
    Task HandleInvoicePaidAsync(StripeInvoiceDto invoice, CancellationToken cancellationToken);

    /// <summary>
    ///     Processes an updated subscription notification.
    /// </summary>
    /// <param name="subscription">The subscription payload extracted from the webhook.</param>
    /// <param name="cancellationToken">Token used to signal cancellation.</param>
    Task HandleSubscriptionUpdatedAsync(StripeSubscriptionDto subscription, CancellationToken cancellationToken);
}
