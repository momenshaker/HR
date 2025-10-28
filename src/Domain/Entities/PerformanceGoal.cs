namespace HR.Domain.Entities;

/// <summary>
///     Represents a cascaded goal within a performance review hierarchy.
/// </summary>
public sealed class PerformanceGoal
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public decimal Weight { get; init; }

    public Guid? ParentGoalId { get; init; }

    public string Alignment { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;
}
