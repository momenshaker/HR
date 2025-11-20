using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkCustomerRepository : EntityFrameworkRepository<Customer>, ICustomerRepository
{
    public EntityFrameworkCustomerRepository(HrDbContext dbContext)
        : base(dbContext, customer => customer.Id)
    {
    }

    public Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(customer, cancellationToken);
    }

    public Task<Customer?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(customerId, cancellationToken);
    }

    public async Task<bool> ExistsByBillingEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var normalized = email.Trim();
        return await DbContext.Customers
            .AsNoTracking()
            .AnyAsync(customer => customer.BillingEmail.ToUpper() == normalized.ToUpper(), cancellationToken)
            .ConfigureAwait(false);
    }
}
