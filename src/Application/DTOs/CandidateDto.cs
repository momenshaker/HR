using System.Collections.Generic;

namespace HR.Application.DTOs;

/// <summary>
///     Read model representing a recruitment candidate.
/// </summary>
public sealed record CandidateDto(
    Guid Id,
    string FullName,
    string Email,
    string Phone,
    string CurrentCompany,
    string CurrentTitle,
    int? YearsOfExperience,
    string AppliedRole,
    string Stage,
    string Source,
    string LinkedInProfileUrl,
    DateTime AppliedAtUtc,
    DateTime? NextInterviewAtUtc,
    string ResumeUrl,
    string Notes,
    IReadOnlyCollection<string> Tags,
    bool IsInTalentPool);
