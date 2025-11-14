using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Payload to register a holiday.
/// </summary>
public sealed class CreateHolidayRequest : IValidatableRequest
{
    [Required]
    public Guid OrganizationId { get; init; }

    [Required]
    public DateOnly Date { get; init; }

    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    public bool IsPaid { get; init; }

    [MaxLength(10)]
    public string CountryCode { get; init; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; init; } = string.Empty;
}
