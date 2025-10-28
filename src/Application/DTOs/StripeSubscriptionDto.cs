namespace HR.Application.DTOs;

/// <summary>
///     Represents subscription metadata emitted from Stripe webhooks.
/// </summary>
public sealed record StripeSubscriptionDto
{
    /// <summary>
    ///     Gets the unique identifier of the subscription.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the identifier of the customer associated with the subscription.
    /// </summary>
    public string? CustomerId { get; init; };

    /// <summary>
    ///     Gets the current status of the subscription (e.g. active, past_due).
    /// </summary>
    public string? Status { get; init; };

    /// <summary>
    ///     Gets the end of the current billing period expressed as a Unix timestamp, when available.
    /// </summary>
    public long? CurrentPeriodEnd { get; init; };
}

