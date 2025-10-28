namespace HR.Domain.Entities;

/// <summary>
///     Represents a billing invoice generated for a customer.
/// </summary>
public sealed class Invoice
{
    public Guid Id { get; init; }

    public Guid CustomerId { get; init; }

    public Guid? SubscriptionId { get; init; }

    public string InvoiceNumber { get; init; } = string.Empty;

    public DateOnly IssueDate { get; init; }

    public DateOnly DueDate { get; init; }

    public DateOnly? PaidDate { get; init; }

    public decimal Subtotal { get; init; }

    public decimal TaxTotal { get; init; }

    public decimal Total { get; init; }

    public decimal AmountPaid { get; init; }

    public string Currency { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string Notes { get; init; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; init; }

    public DateTimeOffset? UpdatedAtUtc { get; init; }
}
