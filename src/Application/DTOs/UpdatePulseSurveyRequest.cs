namespace HR.Application.DTOs;

/// <summary>
///     Request payload for updating a pulse survey.
/// </summary>
public sealed class UpdatePulseSurveyRequest
{
    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public string QuestionSet { get; init; } = string.Empty;

    public int ResponseWindowMinutes { get; init; }

    public DateTime LaunchDateUtc { get; init; }

    public DateTime? CloseDateUtc { get; init; }

    public Guid OwnerId { get; init; }
}
