namespace HR.Domain.Entities;

public sealed record RatingScale
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public decimal MinScore { get; init; }

    public decimal MaxScore { get; init; }

    public bool AllowHalfPoints { get; init; }

    public IReadOnlyCollection<RatingScaleLevel> Levels { get; init; } = Array.Empty<RatingScaleLevel>();
}

public sealed record RatingScaleLevel
{
    public Guid Id { get; init; }

    public Guid RatingScaleId { get; init; }

    public decimal Score { get; init; }

    public string Label { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;
}
