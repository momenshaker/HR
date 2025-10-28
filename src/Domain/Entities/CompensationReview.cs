namespace HR.Domain.Entities;

/// <summary>
///     Represents compensation recommendations tied to a performance review.
/// </summary>
public sealed class CompensationReview
{
    public DateOnly EffectiveDate { get; init; }

    public decimal CurrentBaseSalary { get; init; }

    public decimal ProposedBaseSalary { get; init; }

    public decimal BonusRecommendation { get; init; }

    public string Currency { get; init; } = string.Empty;

    public string Notes { get; init; } = string.Empty;
}
