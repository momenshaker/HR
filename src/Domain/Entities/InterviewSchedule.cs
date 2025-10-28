using System;
using System.Collections.Generic;

namespace HR.Domain.Entities;

/// <summary>
///     Represents a scheduled interview for a candidate.
/// </summary>
public sealed class InterviewSchedule
{
    public Guid Id { get; init; }

    public Guid CandidateId { get; init; }

    public Guid VacancyId { get; init; }

    public string Stage { get; init; } = string.Empty;

    public DateTime ScheduledAtUtc { get; init; }

    public TimeSpan Duration { get; init; }

    public string Mode { get; init; } = string.Empty;

    public string Location { get; init; } = string.Empty;

    public string MeetingLink { get; init; } = string.Empty;

    public List<string> Interviewers { get; init; } = new();

    public string Status { get; init; } = string.Empty;

    public string Notes { get; init; } = string.Empty;
}
