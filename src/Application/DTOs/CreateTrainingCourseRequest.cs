using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for creating a training course.
/// </summary>
public sealed class CreateTrainingCourseRequest : IValidatableRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; init; } = string.Empty;

    [MaxLength(100)]
    public string Category { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; init; } = string.Empty;

    [MaxLength(150)]
    public string Instructor { get; init; } = string.Empty;

    [Required]
    public DateOnly StartDate { get; init; }

    [Required]
    public DateOnly EndDate { get; init; }

    [Range(0, 10000)]
    public int Capacity { get; init; }

    [MaxLength(50)]
    public string DeliveryMode { get; init; } = string.Empty;

    [MaxLength(20)]
    public string SkillLevel { get; init; } = string.Empty;

    [Range(0, 1000)]
    public int DurationHours { get; init; }

    [MaxLength(1000)]
    public string CertificationCriteria { get; init; } = string.Empty;

    public IList<string> CompetencyCodes { get; init; } = new List<string>();

    public bool OffersCertification { get; init; }
}