namespace HR.Domain.Entities;

/// <summary>
///     Represents the payment status of an invoice.
/// </summary>
public enum InvoiceStatus
{
    Pending,
    Paid,
    Failed,
    PastDue
}
