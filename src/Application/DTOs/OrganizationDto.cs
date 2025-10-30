namespace HR.Application.DTOs;

/// <summary>
///     Read model representing an organization.
/// </summary>
public sealed record OrganizationDto(
    Guid Id,
    string Name,
    string Code,
    string Description,
    bool IsActive);
