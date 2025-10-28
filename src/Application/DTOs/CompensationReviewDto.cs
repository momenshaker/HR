namespace HR.Application.DTOs;

/// <summary>
///     Read model representing compensation outcomes from a review.
/// </summary>
public sealed record CompensationReviewDto(
    DateOnly EffectiveDate,
    decimal CurrentBaseSalary,
    decimal ProposedBaseSalary,
    decimal BonusRecommendation,
    string Currency,
    string Notes);
