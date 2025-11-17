using System;
using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming payload for creating an employment offer.
/// </summary>
public sealed class CreateOfferRequest : IValidatableRequest
{
    [Required]
    public Guid ApplicationId { get; init; }

    [Required]
    [MaxLength(200)]
    public string PositionTitle { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string EmploymentType { get; init; } = string.Empty;

    public decimal? ProposedSalary { get; init; }

    [MaxLength(10)]
    public string Currency { get; init; } = string.Empty;

    public DateTime? StartDate { get; init; }

    [Range(0, 24)]
    public int? ProbationPeriodMonths { get; init; }

    [MaxLength(50)]
    public string Status { get; init; } = string.Empty;

    [Url]
    public string OfferDocumentUrl { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string Comments { get; init; } = string.Empty;
}
