using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.Common.Exceptions;
using HR.Application.DTOs;
using HR.Domain.Entities;
using HR.Application.Mappings;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    /// <inheritdoc />
    public async Task<Customer> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var normalizedEmail = request.BillingEmail.Trim();
        if (await _customerRepository.ExistsByBillingEmailAsync(normalizedEmail, cancellationToken).ConfigureAwait(false))
        {
            throw new UniqueConstraintViolationException("Customer", "BillingEmail", normalizedEmail);
        }

        var entity = request.ToEntity();
        return await _customerRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
    }
}
