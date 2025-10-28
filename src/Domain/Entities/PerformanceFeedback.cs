namespace HR.Domain.Entities;

/// <summary>
///     Represents structured feedback collected during a performance cycle.
/// </summary>
public sealed class PerformanceFeedback
{
    public Guid Id { get; init; }

    public string FeedbackType { get; init; } = string.Empty;

    public string Comments { get; init; } = string.Empty;

    public Guid SubmittedBy { get; init; }

    public DateTime SubmittedAtUtc { get; init; }
}
