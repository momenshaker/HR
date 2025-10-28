namespace HR.Domain.Entities;

/// <summary>
///     Represents a key performance indicator tracked within a review.
/// </summary>
public sealed class PerformanceKpi
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public decimal TargetValue { get; init; }

    public decimal ActualValue { get; init; }

    public string UnitOfMeasure { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;
}
