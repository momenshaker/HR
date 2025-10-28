using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service abstraction for working with subscription invoices.
/// </summary>
public interface IInvoiceService
{
    Task<InvoiceDto> CreateAsync(Guid subscriptionId, decimal amountDue, string currency, DateTime dueDate, CancellationToken cancellationToken = default);

    Task<InvoiceDto?> GetLatestAsync(Guid subscriptionId, CancellationToken cancellationToken = default);

    Task<bool> MarkPaidAsync(Guid invoiceId, CancellationToken cancellationToken = default);
}
