namespace HR.Application.DTOs;

/// <summary>
///     Read model representing a holiday that affects attendance expectations.
/// </summary>
public sealed record HolidayDto(
    Guid Id,
    Guid OrganizationId,
    DateOnly Date,
    string Name,
    bool IsPaid,
    string CountryCode,
    string Description);
