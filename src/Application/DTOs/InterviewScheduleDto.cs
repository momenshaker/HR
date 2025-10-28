namespace HR.Application.DTOs;

/// <summary>
///     Read model describing a scheduled interview.
/// </summary>
public sealed record InterviewScheduleDto(
    Guid Id,
    Guid CandidateId,
    Guid VacancyId,
    string Stage,
    DateTime ScheduledAtUtc,
    int DurationMinutes,
    string Mode,
    string Location,
    string MeetingLink,
    IReadOnlyCollection<string> Interviewers,
    string Status,
    string Notes);
