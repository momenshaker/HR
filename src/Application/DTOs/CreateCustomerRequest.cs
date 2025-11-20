using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Payload used to create a new customer record in the billing system.
/// </summary>
public sealed class CreateCustomerRequest : IValidatableRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(320)]
    public string BillingEmail { get; init; } = string.Empty;

    [MaxLength(30)]
    public string BillingPhone { get; init; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string AddressLine1 { get; init; } = string.Empty;

    [MaxLength(200)]
    public string AddressLine2 { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string City { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string State { get; init; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string PostalCode { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Country { get; init; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; init; } = "Active";

    [Range(0, 365)]
    public int? TrialPeriodDays { get; init; }
}
