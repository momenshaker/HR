namespace HR.Application.DTOs;

/// <summary>
///     Read model representing a recruitment candidate.
/// </summary>
public sealed record CandidateDto(
    Guid Id,
    string FullName,
    string Email,
    string AppliedRole,
    string Stage,
    string Source,
    DateTime AppliedAtUtc,
    DateTime? NextInterviewAtUtc,
    string ResumeUrl,
    string Notes);
