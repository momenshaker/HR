using System;
using System.Collections.Generic;
using System.Linq;
using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="Candidate" /> entities.
/// </summary>
public static class CandidateMappings
{
    public static CandidateDto ToDto(this Candidate candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        return new CandidateDto(
            candidate.Id,
            candidate.FullName,
            candidate.Email,
            candidate.Phone,
            candidate.CurrentCompany,
            candidate.CurrentTitle,
            candidate.YearsOfExperience,
            candidate.AppliedRole,
            candidate.Stage,
            candidate.Source,
            candidate.LinkedInProfileUrl,
            candidate.AppliedAtUtc,
            candidate.NextInterviewAtUtc,
            candidate.ResumeUrl,
            candidate.Notes,
            candidate.Tags,
            candidate.IsInTalentPool);
    }

    public static Candidate ToEntity(this CreateCandidateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new Candidate
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            Phone = request.Phone.Trim(),
            CurrentCompany = request.CurrentCompany.Trim(),
            CurrentTitle = request.CurrentTitle.Trim(),
            YearsOfExperience = request.YearsOfExperience,
            AppliedRole = request.AppliedRole.Trim(),
            Stage = request.Stage.Trim(),
            Source = request.Source.Trim(),
            LinkedInProfileUrl = request.LinkedInProfileUrl.Trim(),
            AppliedAtUtc = DateTime.UtcNow,
            NextInterviewAtUtc = request.NextInterviewAtUtc,
            ResumeUrl = request.ResumeUrl.Trim(),
            Notes = request.Notes.Trim(),
            Tags = Normalize(request.Tags),
            IsInTalentPool = request.IsInTalentPool
        };
    }

    public static Candidate ApplyUpdates(this UpdateCandidateRequest request, Candidate existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        return new Candidate
        {
            Id = existing.Id,
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            Phone = request.Phone.Trim(),
            CurrentCompany = request.CurrentCompany.Trim(),
            CurrentTitle = request.CurrentTitle.Trim(),
            YearsOfExperience = request.YearsOfExperience,
            AppliedRole = request.AppliedRole.Trim(),
            Stage = request.Stage.Trim(),
            Source = request.Source.Trim(),
            LinkedInProfileUrl = request.LinkedInProfileUrl.Trim(),
            AppliedAtUtc = existing.AppliedAtUtc,
            NextInterviewAtUtc = request.NextInterviewAtUtc,
            ResumeUrl = request.ResumeUrl.Trim(),
            Notes = request.Notes.Trim(),
            Tags = Normalize(request.Tags),
            IsInTalentPool = request.IsInTalentPool
        };
    }

    private static List<string> Normalize(IReadOnlyCollection<string>? values)
    {
        if (values is null || values.Count == 0)
        {
            return new List<string>();
        }

        return values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
