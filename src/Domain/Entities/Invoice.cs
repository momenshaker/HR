namespace HR.Domain.Entities;

/// <summary>
///     Represents a billing invoice associated with a subscription.
/// </summary>
public sealed class Invoice
{
    public Invoice(
        Guid id,
        Guid subscriptionId,
        decimal amountDue,
        string currency,
        DateTime dueDate,
        InvoiceStatus status,
        Uri? hostedInvoiceUrl,
        Uri? pdfUrl,
        DateTime createdAtUtc)
    {
        Id = id;
        SubscriptionId = subscriptionId;
        AmountDue = amountDue;
        Currency = currency;
        DueDate = dueDate;
        Status = status;
        HostedInvoiceUrl = hostedInvoiceUrl;
        PdfUrl = pdfUrl;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }

    public Guid SubscriptionId { get; }

    public decimal AmountDue { get; private set; }

    public string Currency { get; private set; }

    public DateTime DueDate { get; private set; }

    public InvoiceStatus Status { get; private set; }

    public Uri? HostedInvoiceUrl { get; private set; }

    public Uri? PdfUrl { get; private set; }

    public DateTime CreatedAtUtc { get; }

    public DateTime? PaidAtUtc { get; private set; }

    public void MarkPaid(DateTime paidAtUtc)
    {
        Status = InvoiceStatus.Paid;
        PaidAtUtc = paidAtUtc;
    }

    public void MarkFailed()
    {
        Status = InvoiceStatus.Failed;
        PaidAtUtc = null;
    }

    public void MarkPastDue()
    {
        Status = InvoiceStatus.PastDue;
    }

    public void UpdateAmount(decimal amountDue, string currency)
    {
        AmountDue = amountDue;
        Currency = currency;
    }

    public void UpdateDueDate(DateTime dueDate)
    {
        DueDate = dueDate;
    }

    public void UpdateHostedInvoiceUrl(Uri? hostedInvoiceUrl)
    {
        HostedInvoiceUrl = hostedInvoiceUrl;
    }

    public void UpdatePdfUrl(Uri? pdfUrl)
    {
        PdfUrl = pdfUrl;
    }
}
