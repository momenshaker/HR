namespace HR.Application.DTOs;

/// <summary>
///     Data transfer object for pulse survey information.
/// </summary>
public sealed record PulseSurveyDto(
    Guid Id,
    string Title,
    string Description,
    string Audience,
    string QuestionSet,
    int ResponseWindowMinutes,
    DateTime LaunchDateUtc,
    DateTime? CloseDateUtc,
    Guid OwnerId);
