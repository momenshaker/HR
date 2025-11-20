using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository contract for persisting <see cref="Customer" /> aggregates.
/// </summary>
public interface ICustomerRepository
{
    Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default);

    Task<Customer?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByBillingEmailAsync(string email, CancellationToken cancellationToken = default);
}
