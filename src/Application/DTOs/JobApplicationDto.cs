using System;

namespace HR.Application.DTOs;

/// <summary>
///     Read model describing a job application linked to a posting.
/// </summary>
public sealed record JobApplicationDto(
    Guid Id,
    Guid CandidateId,
    Guid JobPostingId,
    DateTime AppliedDate,
    string CurrentStage,
    string Status,
    string Source,
    string CVUrl,
    string CoverLetter,
    decimal? ExpectedSalary,
    string NoticePeriod,
    decimal? OverallScore);
