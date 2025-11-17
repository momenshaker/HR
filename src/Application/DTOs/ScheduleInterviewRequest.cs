using System;
using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming payload for creating a scheduled interview.
/// </summary>
public sealed class ScheduleInterviewRequest
{
    [Required]
    public Guid CandidateId { get; init; }

    [Required]
    public Guid VacancyId { get; init; }

    public Guid? ApplicationId { get; init; }

    public Guid? StageId { get; init; }

    public Guid? ScheduledById { get; init; }

    [Required]
    [MaxLength(100)]
    public string Stage { get; init; } = string.Empty;

    [Required]
    public DateTime ScheduledAtUtc { get; init; }

    [Range(15, 480)]
    public int DurationMinutes { get; init; } = 60;

    [MaxLength(50)]
    public string Mode { get; init; } = string.Empty;

    [MaxLength(200)]
    public string Location { get; init; } = string.Empty;

    [MaxLength(200)]
    public string MeetingLink { get; init; } = string.Empty;

    public IReadOnlyCollection<string> Interviewers { get; init; } = Array.Empty<string>();

    [MaxLength(1000)]
    public string Notes { get; init; } = string.Empty;
}
