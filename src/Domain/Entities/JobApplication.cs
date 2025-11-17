using System;

namespace HR.Domain.Entities;

/// <summary>
///     Represents a candidate applying to a specific job posting.
/// </summary>
public sealed class JobApplication
{
    public Guid Id { get; init; }

    public Guid CandidateId { get; init; }

    public Guid JobPostingId { get; init; }

    public DateTime AppliedDate { get; init; }

    public string CurrentStage { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string Source { get; init; } = string.Empty;

    public string CVUrl { get; init; } = string.Empty;

    public string CoverLetter { get; init; } = string.Empty;

    public decimal? ExpectedSalary { get; init; }

    public string NoticePeriod { get; init; } = string.Empty;

    public decimal? OverallScore { get; init; }
}
