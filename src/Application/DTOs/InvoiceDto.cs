namespace HR.Application.DTOs;

/// <summary>
///     Read model representing an invoice.
/// </summary>
public sealed record InvoiceDto(
    Guid Id,
    Guid SubscriptionId,
    decimal AmountDue,
    string Currency,
    DateTime DueDate,
    string Status,
    string? HostedInvoiceUrl,
    string? PdfUrl,
    DateTime CreatedAt,
    DateTime? PaidAt);
