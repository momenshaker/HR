namespace HR.Domain.Entities;

/// <summary>
///     Represents the payment status of an invoice.
/// </summary>
public enum InvoiceStatus
{
    Draft = 0,
    Open = 1,
    PastDue = 2,
    Paid = 3,
    Failed = 4,
    Void = 5,
    Refunded = 6
}
