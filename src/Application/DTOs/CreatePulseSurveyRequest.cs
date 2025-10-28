namespace HR.Application.DTOs;

/// <summary>
///     Request payload for creating a pulse survey.
/// </summary>
public sealed class CreatePulseSurveyRequest
{
    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public string QuestionSet { get; init; } = string.Empty;

    public int ResponseWindowMinutes { get; init; }

    public DateTime LaunchDateUtc { get; init; } = DateTime.UtcNow;

    public DateTime? CloseDateUtc { get; init; }

    public Guid OwnerId { get; init; }
}
