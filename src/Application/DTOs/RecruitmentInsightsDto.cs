namespace HR.Application.DTOs;

/// <summary>
///     Aggregated insights for collaborative hiring.
/// </summary>
public sealed record RecruitmentInsightsDto(
    int TotalVacancies,
    int OpenVacancies,
    int ActiveCandidates,
    IReadOnlyCollection<PipelineStageSummaryDto> PipelineStageSummaries,
    IReadOnlyCollection<UpcomingInterviewSummaryDto> UpcomingInterviews,
    IReadOnlyCollection<HiringTeamContributorDto> HiringTeamCollaborators);

/// <summary>
///     Summary information about a pipeline stage.
/// </summary>
public sealed record PipelineStageSummaryDto(string Stage, int CandidateCount);

/// <summary>
///     Lightweight overview of an upcoming interview.
/// </summary>
public sealed record UpcomingInterviewSummaryDto(
    Guid InterviewId,
    Guid CandidateId,
    Guid VacancyId,
    DateTime ScheduledAtUtc,
    string Stage,
    IReadOnlyCollection<string> Interviewers);

/// <summary>
///     Describes hiring team participation across vacancies.
/// </summary>
public sealed record HiringTeamContributorDto(string Name, int VacancyCount);
