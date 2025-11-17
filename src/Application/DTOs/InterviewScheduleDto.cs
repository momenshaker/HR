using System;
using System.Collections.Generic;

namespace HR.Application.DTOs;

/// <summary>
///     Read model describing a scheduled interview.
/// </summary>
public sealed record InterviewScheduleDto(
    Guid Id,
    Guid CandidateId,
    Guid VacancyId,
    Guid? ApplicationId,
    Guid? StageId,
    string Stage,
    Guid? ScheduledById,
    DateTime ScheduledAtUtc,
    int DurationMinutes,
    string Mode,
    string Location,
    string MeetingLink,
    IReadOnlyCollection<string> Interviewers,
    string Status,
    string Notes);
