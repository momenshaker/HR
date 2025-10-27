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
            candidate.AppliedRole,
            candidate.Stage,
            candidate.Source,
            candidate.AppliedAtUtc,
            candidate.NextInterviewAtUtc,
            candidate.ResumeUrl,
            candidate.Notes);
    }

    public static Candidate ToEntity(this CreateCandidateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new Candidate
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            AppliedRole = request.AppliedRole.Trim(),
            Stage = request.Stage.Trim(),
            Source = request.Source.Trim(),
            AppliedAtUtc = DateTime.UtcNow,
            NextInterviewAtUtc = request.NextInterviewAtUtc,
            ResumeUrl = request.ResumeUrl.Trim(),
            Notes = request.Notes.Trim()
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
            AppliedRole = request.AppliedRole.Trim(),
            Stage = request.Stage.Trim(),
            Source = request.Source.Trim(),
            AppliedAtUtc = existing.AppliedAtUtc,
            NextInterviewAtUtc = request.NextInterviewAtUtc,
            ResumeUrl = request.ResumeUrl.Trim(),
            Notes = request.Notes.Trim()
        };
    }
}
