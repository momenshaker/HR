using System.ComponentModel.DataAnnotations;
using HR.Application.Common.Validation;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Aggregated payload used during the anonymous customer onboarding flow.
/// </summary>
public sealed class OnboardingRequest : IValidatableRequest
{
    [ValidateComplexType]
    public OnboardingAccountInfo Account { get; set; } = new();

    [ValidateComplexType]
    public OnboardingOrganizationInfo Organization { get; set; } = new();

    [ValidateComplexType]
    public OnboardingSubscriptionSelection Subscription { get; set; } = new();
}

/// <summary>
///     Represents the credentials and personal details for the first administrator.
/// </summary>
public sealed class OnboardingAccountInfo
{
    [Required]
    [EmailAddress]
    [MaxLength(320)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    [MaxLength(128)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(30)]
    public string PhoneNumber { get; set; } = string.Empty;
}

/// <summary>
///     Captures the organization and billing details for the new tenant.
/// </summary>
public sealed class OnboardingOrganizationInfo
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(150)]
    public string Industry { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Region { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string HeadquartersAddress { get; set; } = string.Empty;

    [MaxLength(50)]
    public string TimeZone { get; set; } = string.Empty;

    [EmailAddress]
    [MaxLength(150)]
    public string PrimaryContactEmail { get; set; } = string.Empty;

    [Url]
    [MaxLength(200)]
    public string WebsiteUrl { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string BillingAddressLine1 { get; set; } = string.Empty;

    [MaxLength(200)]
    public string BillingAddressLine2 { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string BillingCity { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string BillingState { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string BillingPostalCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string BillingCountry { get; set; } = string.Empty;

    [MaxLength(30)]
    public string BillingPhone { get; set; } = string.Empty;
}

/// <summary>
///     Describes the subscription plan selection made during onboarding.
/// </summary>
public sealed class OnboardingSubscriptionSelection
{
    [Required]
    public Guid PlanId { get; set; }

    [Range(1, int.MaxValue)]
    public int Seats { get; set; } = 1;

    [Range(0, 365)]
    public int? TrialPeriodDays { get; set; }
}
