namespace HR.Domain.Entities;

using System;

/// <summary>
/// Represents a billing invoice generated for a customer and/or a subscription,
/// including amounts, lifecycle status, and helpful links (hosted invoice / PDF).
/// </summary>
public sealed class Invoice
{
    /// <summary>
    /// Creates a new invoice.
    /// </summary>
    /// <param name="id">Invoice identifier.</param>
    /// <param name="customerId">Customer identifier.</param>
    /// <param name="subscriptionId">Related subscription identifier, if any.</param>
    /// <param name="invoiceNumber">Human-readable invoice number.</param>
    /// <param name="currency">ISO 4217 currency code (e.g., "USD").</param>
    /// <param name="issueDate">Invoice issue date (no time component).</param>
    /// <param name="dueDate">Invoice due date (no time component).</param>
    /// <param name="subtotal">Subtotal before tax.</param>
    /// <param name="taxTotal">Total tax amount.</param>
    /// <param name="status">Initial invoice status.</param>
    /// <param name="createdAtUtc">Creation timestamp (UTC).</param>
    public Invoice(
        Guid id,
        Guid customerId,
        Guid? subscriptionId,
        string invoiceNumber,
        string currency,
        DateOnly issueDate,
        DateOnly dueDate,
        decimal subtotal,
        decimal taxTotal,
        InvoiceStatus status,
        DateTimeOffset createdAtUtc)
    {
        Id = id;
        CustomerId = customerId;
        SubscriptionId = subscriptionId;
        InvoiceNumber = invoiceNumber ?? string.Empty;
        Currency = currency ?? string.Empty;
        IssueDate = issueDate;
        DueDate = dueDate;
        Subtotal = EnsureNonNegative(subtotal);
        TaxTotal = EnsureNonNegative(taxTotal);
        Status = status;
        CreatedAtUtc = createdAtUtc;
    }

    /// <summary>Invoice identifier.</summary>
    public Guid Id { get; }

    /// <summary>Customer identifier.</summary>
    public Guid CustomerId { get; private set; }

    /// <summary>Related subscription identifier, if any.</summary>
    public Guid? SubscriptionId { get; private set; }

    /// <summary>Human-readable invoice number.</summary>
    public string InvoiceNumber { get; private set; } = string.Empty;

    /// <summary>Date the invoice was issued.</summary>
    public DateOnly IssueDate { get; private set; }

    /// <summary>Date payment is due.</summary>
    public DateOnly DueDate { get; private set; }

    /// <summary>UTC timestamp when fully paid, if paid.</summary>
    public DateTimeOffset? PaidAtUtc { get; private set; }

    /// <summary>Convenience: paid calendar date (if paid).</summary>
    public DateOnly? PaidDate => PaidAtUtc?.Date;

    /// <summary>Subtotal before taxes.</summary>
    public decimal Subtotal { get; private set; }

    /// <summary>Total tax amount.</summary>
    public decimal TaxTotal { get; private set; }

    /// <summary>Total (Subtotal + TaxTotal).</summary>
    public decimal Total => Subtotal + TaxTotal;

    /// <summary>Cumulative amount paid.</summary>
    public decimal AmountPaid { get; private set; }

    /// <summary>Amount still due (Total - AmountPaid), never below zero.</summary>
    public decimal AmountDue => Math.Max(0, Total - AmountPaid);

    /// <summary>ISO 4217 currency code.</summary>
    public string Currency { get; private set; } = string.Empty;

    /// <summary>Invoice lifecycle status.</summary>
    public InvoiceStatus Status { get; private set; }

    /// <summary>Freeform notes.</summary>
    public string Notes { get; private set; } = string.Empty;

    /// <summary>Link to hosted invoice, if provided by the PSP/Billing system.</summary>
    public Uri? HostedInvoiceUrl { get; private set; }

    /// <summary>Link to invoice PDF, if available.</summary>
    public Uri? PdfUrl { get; private set; }

    /// <summary>Creation timestamp (UTC).</summary>
    public DateTimeOffset CreatedAtUtc { get; }

    /// <summary>Last update timestamp (UTC).</summary>
    public DateTimeOffset? UpdatedAtUtc { get; private set; }

    // ---------------------------
    // Domain behavior / mutators
    // ---------------------------

    /// <summary>Marks the invoice as paid and sets the paid timestamp.</summary>
    public void MarkPaid(DateTimeOffset paidAtUtc)
    {
        Status = InvoiceStatus.Paid;
        PaidAtUtc = paidAtUtc;
        UpdatedNow();
    }

    /// <summary>Marks the invoice as failed (e.g., payment failure).</summary>
    public void MarkFailed()
    {
        Status = InvoiceStatus.Failed;
        PaidAtUtc = null;
        UpdatedNow();
    }

    /// <summary>Marks the invoice as past due.</summary>
    public void MarkPastDue()
    {
        Status = InvoiceStatus.PastDue;
        UpdatedNow();
    }

    /// <summary>Voids the invoice (no longer payable).</summary>
    public void MarkVoid()
    {
        Status = InvoiceStatus.Void;
        UpdatedNow();
    }

    /// <summary>Adjusts the due date.</summary>
    public void UpdateDueDate(DateOnly dueDate)
    {
        DueDate = dueDate;
        UpdatedNow();
    }

    /// <summary>Updates hosted invoice URL.</summary>
    public void UpdateHostedInvoiceUrl(Uri? hostedInvoiceUrl)
    {
        HostedInvoiceUrl = hostedInvoiceUrl;
        UpdatedNow();
    }

    /// <summary>Updates PDF URL.</summary>
    public void UpdatePdfUrl(Uri? pdfUrl)
    {
        PdfUrl = pdfUrl;
        UpdatedNow();
    }

    /// <summary>Updates notes.</summary>
    public void UpdateNotes(string? notes)
    {
        Notes = notes ?? string.Empty;
        UpdatedNow();
    }

    /// <summary>Changes currency code.</summary>
    public void UpdateCurrency(string currency)
    {
        Currency = currency ?? string.Empty;
        UpdatedNow();
    }

    /// <summary>Replaces monetary breakdown (subtotal/tax). Recalculates derived totals.</summary>
    public void UpdateAmounts(decimal subtotal, decimal taxTotal)
    {
        Subtotal = EnsureNonNegative(subtotal);
        TaxTotal = EnsureNonNegative(taxTotal);
        UpdatedNow();
    }

    /// <summary>Applies a payment to the invoice. Does not allow negative or over-refund.</summary>
    public void ApplyPayment(decimal amount, DateTimeOffset appliedAtUtc)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Payment amount must be positive.");
        AmountPaid = Math.Min(Total, AmountPaid + amount);

        if (AmountDue == 0 && Status != InvoiceStatus.Paid)
        {
            MarkPaid(appliedAtUtc);
        }
        else
        {
            UpdatedNow();
        }
    }

    /// <summary>Sets customer/subscription identifiers (e.g., when linking after creation).</summary>
    public void LinkEntities(Guid customerId, Guid? subscriptionId)
    {
        CustomerId = customerId;
        SubscriptionId = subscriptionId;
        UpdatedNow();
    }

    /// <summary>Updates the human-readable invoice number.</summary>
    public void UpdateInvoiceNumber(string invoiceNumber)
    {
        InvoiceNumber = invoiceNumber ?? string.Empty;
        UpdatedNow();
    }

    // ---------------------------
    // Helpers
    // ---------------------------

    private void UpdatedNow() => UpdatedAtUtc = DateTimeOffset.UtcNow;

    private static decimal EnsureNonNegative(decimal value) =>
        value < 0 ? throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be negative.") : value;
}

