namespace HR.Infrastructure.Options;

/// <summary>
///     Configuration options for Stripe webhook validation.
/// </summary>
public sealed class StripeWebhookOptions
{
    /// <summary>
    ///     Gets the configuration section name containing Stripe webhook settings.
    /// </summary>
    public const string SectionName = "Billing:Stripe";

    /// <summary>
    ///     Gets or sets the signing secret assigned by Stripe to the webhook endpoint.
    /// </summary>
    public string EndpointSecret { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the tolerated clock skew, expressed in seconds, between the incoming timestamp and the local clock.
    /// </summary>
    public int ToleranceInSeconds { get; set; } = 300;
}

