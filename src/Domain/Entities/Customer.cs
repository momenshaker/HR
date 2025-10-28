namespace HR.Domain.Entities;

/// <summary>
///     Represents a paying organization or account that holds subscriptions in the billing system.
/// </summary>
public sealed class Customer
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string BillingEmail { get; init; } = string.Empty;

    public string BillingPhone { get; init; } = string.Empty;

    public string AddressLine1 { get; init; } = string.Empty;

    public string AddressLine2 { get; init; } = string.Empty;

    public string City { get; init; } = string.Empty;

    public string State { get; init; } = string.Empty;

    public string PostalCode { get; init; } = string.Empty;

    public string Country { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public DateOnly? TrialEndsOn { get; init; }

    public DateTimeOffset CreatedAtUtc { get; init; }

    public DateTimeOffset? UpdatedAtUtc { get; init; }
}
