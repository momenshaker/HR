using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Service used to manage customer billing records.
/// </summary>
public interface ICustomerService
{
    Task<Customer> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
}
