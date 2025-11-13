namespace HR.Application.DTOs;

/// <summary>
///     Read model representing an organization.
/// </summary>
public sealed record OrganizationDto(
    Guid Id,
    string Name,
    string Code,
    string Description,
    string Industry,
    string Region,
    string HeadquartersAddress,
    string TimeZone,
    string PrimaryContactEmail,
    string WebsiteUrl,
    bool IsActive);
