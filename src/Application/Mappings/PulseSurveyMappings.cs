using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="PulseSurvey" /> entities.
/// </summary>
public static class PulseSurveyMappings
{
    public static PulseSurveyDto ToDto(this PulseSurvey survey)
    {
        ArgumentNullException.ThrowIfNull(survey);

        return new PulseSurveyDto(
            survey.Id,
            survey.Title,
            survey.Description,
            survey.Audience,
            survey.QuestionSet,
            survey.ResponseWindowMinutes,
            survey.LaunchDateUtc,
            survey.CloseDateUtc,
            survey.OwnerId);
    }

    public static PulseSurvey ToEntity(this CreatePulseSurveyRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new PulseSurvey
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Audience = request.Audience.Trim(),
            QuestionSet = request.QuestionSet.Trim(),
            ResponseWindowMinutes = request.ResponseWindowMinutes,
            LaunchDateUtc = request.LaunchDateUtc,
            CloseDateUtc = request.CloseDateUtc,
            OwnerId = request.OwnerId
        };
    }

    public static PulseSurvey ApplyUpdates(this UpdatePulseSurveyRequest request, PulseSurvey existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        return new PulseSurvey
        {
            Id = existing.Id,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Audience = request.Audience.Trim(),
            QuestionSet = request.QuestionSet.Trim(),
            ResponseWindowMinutes = request.ResponseWindowMinutes,
            LaunchDateUtc = request.LaunchDateUtc,
            CloseDateUtc = request.CloseDateUtc,
            OwnerId = request.OwnerId
        };
    }
}
