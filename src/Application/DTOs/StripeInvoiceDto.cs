namespace HR.Application.DTOs;

/// <summary>
///     Represents the subset of Stripe invoice fields required by the platform.
/// </summary>
public sealed record StripeInvoiceDto
{
    /// <summary>
    ///     Gets the unique identifier of the invoice.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the identifier of the customer associated with the invoice.
    /// </summary>
    public string? CustomerId { get; init; };

    /// <summary>
    ///     Gets the total amount paid on the invoice, expressed in the smallest currency unit.
    /// </summary>
    public long AmountPaid { get; init; };

    /// <summary>
    ///     Gets the three-letter ISO currency code for the invoice.
    /// </summary>
    public string? Currency { get; init; };
}

