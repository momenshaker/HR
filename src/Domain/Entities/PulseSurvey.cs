namespace HR.Domain.Entities;

/// <summary>
///     Represents a lightweight employee pulse survey for capturing sentiment.
/// </summary>
public sealed class PulseSurvey
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public string QuestionSet { get; init; } = string.Empty;

    public int ResponseWindowMinutes { get; init; }

    public DateTime LaunchDateUtc { get; init; }

    public DateTime? CloseDateUtc { get; init; }

    public Guid OwnerId { get; init; }
}
