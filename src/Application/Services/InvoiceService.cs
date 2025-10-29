using System;
using System.Collections.Concurrent;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;
using HR.Domain.Entities;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class InvoiceService : IInvoiceService
{
    private readonly ConcurrentDictionary<Guid, List<Invoice>> _invoices = new();

    public Task<InvoiceDto> CreateAsync(Guid subscriptionId, decimal amountDue, string currency, DateTime dueDate, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency must be supplied.", nameof(currency));
        }

        var invoice = new Invoice(
            id: Guid.NewGuid(),
            customerId: Guid.Empty,
            subscriptionId: subscriptionId,
            invoiceNumber: string.Empty,
            currency: currency,
            issueDate: DateOnly.FromDateTime(DateTime.UtcNow),
            dueDate: DateOnly.FromDateTime(dueDate.Date),
            subtotal: amountDue,
            taxTotal: 0m,
            status: InvoiceStatus.Open,
            createdAtUtc: DateTimeOffset.UtcNow);

        var bucket = _invoices.GetOrAdd(subscriptionId, _ => []);

        lock (bucket)
        {
            bucket.Add(invoice);
        }

        return Task.FromResult(invoice.ToDto());
    }

    public Task<InvoiceDto?> GetLatestAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
    {
        if (!_invoices.TryGetValue(subscriptionId, out var invoices) || invoices.Count == 0)
        {
            return Task.FromResult<InvoiceDto?>(null);
        }

        Invoice latest;
        lock (invoices)
        {
            latest = invoices
                .OrderByDescending(invoice => invoice.DueDate)
                .ThenByDescending(invoice => invoice.CreatedAtUtc)
                .First();
        }

        return Task.FromResult<InvoiceDto?>(latest.ToDto());
    }

    public Task<bool> MarkPaidAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        foreach (var entry in _invoices.Values)
        {
            lock (entry)
            {
                var invoice = entry.FirstOrDefault(item => item.Id == invoiceId);
                if (invoice is null)
                {
                    continue;
                }

                invoice.MarkPaid(DateTimeOffset.UtcNow);
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }
}
