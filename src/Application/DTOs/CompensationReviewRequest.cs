using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Request payload representing compensation recommendations for a review.
/// </summary>
public sealed class CompensationReviewRequest
{
    [Required]
    public DateOnly EffectiveDate { get; init; }

    [Range(0, double.MaxValue)]
    public decimal CurrentBaseSalary { get; init; }

    [Range(0, double.MaxValue)]
    public decimal ProposedBaseSalary { get; init; }

    [Range(0, double.MaxValue)]
    public decimal BonusRecommendation { get; init; }

    [MaxLength(3)]
    public string Currency { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string Notes { get; init; } = string.Empty;
}
