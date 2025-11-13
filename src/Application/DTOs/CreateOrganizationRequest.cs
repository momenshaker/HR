using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming payload for creating an organization.
/// </summary>
public sealed class CreateOrganizationRequest : IValidatableRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Code { get; init; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; init; } = string.Empty;

    [MaxLength(150)]
    public string Industry { get; init; } = string.Empty;

    [MaxLength(100)]
    public string Region { get; init; } = string.Empty;

    [MaxLength(300)]
    public string HeadquartersAddress { get; init; } = string.Empty;

    [MaxLength(50)]
    public string TimeZone { get; init; } = string.Empty;

    [EmailAddress]
    [MaxLength(150)]
    public string PrimaryContactEmail { get; init; } = string.Empty;

    [Url]
    [MaxLength(200)]
    public string WebsiteUrl { get; init; } = string.Empty;

    public bool IsActive { get; init; } = true;
}
