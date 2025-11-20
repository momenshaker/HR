using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

public sealed class InMemoryCustomerRepository : ICustomerRepository
{
    private readonly ConcurrentDictionary<Guid, Customer> _customers = new();

    public Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(customer);

        if (!_customers.TryAdd(customer.Id, customer))
        {
            throw new InvalidOperationException($"A customer with id '{customer.Id}' already exists.");
        }

        return Task.FromResult(customer);
    }

    public Task<Customer?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        _customers.TryGetValue(customerId, out var customer);
        return Task.FromResult(customer);
    }

    public Task<bool> ExistsByBillingEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var normalized = email.Trim();
        var exists = _customers.Values.Any(customer => string.Equals(customer.BillingEmail, normalized, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(exists);
    }
}
