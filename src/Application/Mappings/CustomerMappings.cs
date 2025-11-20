using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="Customer" /> entities.
/// </summary>
public static class CustomerMappings
{
    public static Customer ToEntity(this CreateCustomerRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var now = DateTimeOffset.UtcNow;
        var normalizedStatus = string.IsNullOrWhiteSpace(request.Status) ? "Active" : request.Status.Trim();

        return new Customer
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            BillingEmail = request.BillingEmail.Trim(),
            BillingPhone = request.BillingPhone.Trim(),
            AddressLine1 = request.AddressLine1.Trim(),
            AddressLine2 = request.AddressLine2.Trim(),
            City = request.City.Trim(),
            State = request.State.Trim(),
            PostalCode = request.PostalCode.Trim(),
            Country = request.Country.Trim(),
            Status = normalizedStatus,
            TrialEndsOn = request.TrialPeriodDays.HasValue
                ? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(request.TrialPeriodDays.Value))
                : null,
            CreatedAtUtc = now
        };
    }
}
