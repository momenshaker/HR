using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;
using HR.Domain.Entities;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class RecruitmentService : IRecruitmentService
{
    private readonly ICandidateRepository _candidateRepository;
    private readonly IInterviewScheduleRepository _interviewScheduleRepository;
    private readonly IVacancyRepository _vacancyRepository;

    public RecruitmentService(
        ICandidateRepository candidateRepository,
        IVacancyRepository vacancyRepository,
        IInterviewScheduleRepository interviewScheduleRepository)
    {
        _candidateRepository = candidateRepository;
        _vacancyRepository = vacancyRepository;
        _interviewScheduleRepository = interviewScheduleRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<CandidateDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var candidates = await _candidateRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return candidates.Select(candidate => candidate.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<CandidateDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var candidate = await _candidateRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return candidate?.ToDto();
    }

    /// <inheritdoc />
    public async Task<CandidateDto> CreateAsync(CreateCandidateRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = request.ToEntity();
        var created = await _candidateRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<CandidateDto?> UpdateAsync(Guid id, UpdateCandidateRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _candidateRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updatedEntity = request.ApplyUpdates(existing);
        var persisted = await _candidateRepository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);

        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _candidateRepository.RemoveAsync(id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CandidateDto?> AdvanceCandidateAsync(
        Guid id,
        AdvanceCandidateRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var candidate = await _candidateRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (candidate is null)
        {
            return null;
        }

        var targetStage = request.TargetStage.Trim();
        var nextFollowUp = request.Interview?.ScheduledAtUtc ?? request.NextFollowUpAtUtc;
        var mergedNotes = MergeNotes(candidate.Notes, request.Notes);

        var updatedCandidate = new Candidate
        {
            Id = candidate.Id,
            FullName = candidate.FullName,
            Email = candidate.Email,
            Phone = candidate.Phone,
            CurrentCompany = candidate.CurrentCompany,
            CurrentTitle = candidate.CurrentTitle,
            YearsOfExperience = candidate.YearsOfExperience,
            AppliedRole = candidate.AppliedRole,
            Stage = targetStage,
            Source = candidate.Source,
            LinkedInProfileUrl = candidate.LinkedInProfileUrl,
            AppliedAtUtc = candidate.AppliedAtUtc,
            NextInterviewAtUtc = nextFollowUp,
            ResumeUrl = candidate.ResumeUrl,
            Notes = mergedNotes,
            Tags = candidate.Tags,
            IsInTalentPool = candidate.IsInTalentPool
        };

        var persisted = await _candidateRepository.UpdateAsync(updatedCandidate, cancellationToken).ConfigureAwait(false);
        if (persisted is null)
        {
            return null;
        }

        if (request.Interview is not null)
        {
            var interviewRequest = NormalizeInterviewRequest(id, targetStage, request.Interview);
            await ScheduleInterviewAsync(interviewRequest, cancellationToken).ConfigureAwait(false);
        }

        return persisted.ToDto();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<VacancyDto>> GetVacanciesAsync(CancellationToken cancellationToken = default)
    {
        var vacancies = await _vacancyRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return vacancies.Select(vacancy => vacancy.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<VacancyDto?> GetVacancyByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var vacancy = await _vacancyRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return vacancy?.ToDto();
    }

    /// <inheritdoc />
    public async Task<VacancyDto> PublishVacancyAsync(CreateVacancyRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var vacancy = request.ToEntity();
        var created = await _vacancyRepository.AddAsync(vacancy, cancellationToken).ConfigureAwait(false);

        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<VacancyDto?> UpdateVacancyAsync(
        Guid id,
        UpdateVacancyRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _vacancyRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updated = request.ApplyUpdates(existing);
        var persisted = await _vacancyRepository.UpdateAsync(updated, cancellationToken).ConfigureAwait(false);

        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public async Task<bool> CloseVacancyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await _vacancyRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return false;
        }

        var closingTimestamp = existing.ClosedAtUtc ?? DateTime.UtcNow;
        var closedVacancy = new Vacancy
        {
            Id = existing.Id,
            RequisitionId = existing.RequisitionId,
            PublicTitle = existing.PublicTitle,
            Department = existing.Department,
            Location = existing.Location,
            EmploymentType = existing.EmploymentType,
            WorkMode = existing.WorkMode,
            SalaryVisible = existing.SalaryVisible,
            SalaryRangeText = existing.SalaryRangeText,
            NumberOfPositions = existing.NumberOfPositions,
            PublicDescription = existing.PublicDescription,
            Responsibilities = existing.Responsibilities,
            Requirements = existing.Requirements,
            PostingChannels = existing.PostingChannels,
            PipelineStages = existing.PipelineStages,
            HiringTeam = existing.HiringTeam,
            CreatedAtUtc = existing.CreatedAtUtc,
            PublishedAtUtc = existing.PublishedAtUtc,
            ClosedAtUtc = closingTimestamp,
            Status = string.Equals(existing.Status, "Archived", StringComparison.OrdinalIgnoreCase)
                ? existing.Status
                : "Archived",
            ApplicationUrl = existing.ApplicationUrl
        };

        var persisted = await _vacancyRepository.UpdateAsync(closedVacancy, cancellationToken).ConfigureAwait(false);
        return persisted is not null;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<InterviewScheduleDto>> GetInterviewsAsync(
        Guid? vacancyId = null,
        Guid? candidateId = null,
        bool onlyUpcoming = false,
        CancellationToken cancellationToken = default)
    {
        var interviews = await _interviewScheduleRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var query = interviews.AsEnumerable();

        if (vacancyId.HasValue && vacancyId != Guid.Empty)
        {
            query = query.Where(interview => interview.VacancyId == vacancyId.Value);
        }

        if (candidateId.HasValue && candidateId != Guid.Empty)
        {
            query = query.Where(interview => interview.CandidateId == candidateId.Value);
        }

        if (onlyUpcoming)
        {
            var now = DateTime.UtcNow;
            query = query.Where(interview =>
                string.Equals(interview.Status, "Scheduled", StringComparison.OrdinalIgnoreCase) &&
                interview.ScheduledAtUtc >= now);
        }

        return query.Select(interview => interview.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<InterviewScheduleDto> ScheduleInterviewAsync(
        ScheduleInterviewRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var schedule = request.ToEntity();
        var created = await _interviewScheduleRepository.AddAsync(schedule, cancellationToken).ConfigureAwait(false);

        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<InterviewScheduleDto?> UpdateInterviewAsync(
        Guid id,
        UpdateInterviewScheduleRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _interviewScheduleRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updated = request.ApplyUpdates(existing);
        var persisted = await _interviewScheduleRepository.UpdateAsync(updated, cancellationToken).ConfigureAwait(false);

        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public async Task<bool> CancelInterviewAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await _interviewScheduleRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return false;
        }

        var cancelled = new InterviewSchedule
        {
            Id = existing.Id,
            CandidateId = existing.CandidateId,
            VacancyId = existing.VacancyId,
            Stage = existing.Stage,
            ScheduledAtUtc = existing.ScheduledAtUtc,
            Duration = existing.Duration,
            Mode = existing.Mode,
            Location = existing.Location,
            MeetingLink = existing.MeetingLink,
            Interviewers = existing.Interviewers,
            Status = "Cancelled",
            Notes = existing.Notes
        };

        var persisted = await _interviewScheduleRepository.UpdateAsync(cancelled, cancellationToken).ConfigureAwait(false);
        return persisted is not null;
    }

    /// <inheritdoc />
    public async Task<RecruitmentInsightsDto> GetInsightsAsync(CancellationToken cancellationToken = default)
    {
        var vacanciesTask = _vacancyRepository.GetAllAsync(cancellationToken);
        var candidatesTask = _candidateRepository.GetAllAsync(cancellationToken);
        var interviewsTask = _interviewScheduleRepository.GetAllAsync(cancellationToken);

        await Task.WhenAll(vacanciesTask, candidatesTask, interviewsTask).ConfigureAwait(false);

        var vacancies = await vacanciesTask.ConfigureAwait(false);
        var candidates = await candidatesTask.ConfigureAwait(false);
        var interviews = await interviewsTask.ConfigureAwait(false);

        var stageSummaries = candidates
            .GroupBy(candidate => string.IsNullOrWhiteSpace(candidate.Stage) ? "Unassigned" : candidate.Stage)
            .Select(group => new PipelineStageSummaryDto(group.Key, group.Count()))
            .OrderBy(summary => summary.Stage, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var upcomingInterviews = interviews
            .Where(interview =>
                string.Equals(interview.Status, "Scheduled", StringComparison.OrdinalIgnoreCase) &&
                interview.ScheduledAtUtc >= DateTime.UtcNow)
            .OrderBy(interview => interview.ScheduledAtUtc)
            .Take(10)
            .Select(interview => new UpcomingInterviewSummaryDto(
                interview.Id,
                interview.CandidateId,
                interview.VacancyId,
                interview.ScheduledAtUtc,
                interview.Stage,
                interview.Interviewers))
            .ToArray();

        var hiringCollaborators = vacancies
            .SelectMany(vacancy => vacancy.HiringTeam.Select(member => (member, vacancy.Id)))
            .Where(tuple => !string.IsNullOrWhiteSpace(tuple.member))
            .GroupBy(tuple => tuple.member.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(group => new HiringTeamContributorDto(group.Key, group.Select(tuple => tuple.Id).Distinct().Count()))
            .OrderByDescending(contributor => contributor.VacancyCount)
            .ThenBy(contributor => contributor.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var openVacancies = vacancies.Count(vacancy =>
            string.Equals(vacancy.Status, "Published", StringComparison.OrdinalIgnoreCase));

        return new RecruitmentInsightsDto(
            vacancies.Count,
            openVacancies,
            candidates.Count,
            stageSummaries,
            upcomingInterviews,
            hiringCollaborators);
    }

    private static string MergeNotes(string existingNotes, string incomingNotes)
    {
        var trimmedExisting = existingNotes?.Trim() ?? string.Empty;
        var trimmedIncoming = incomingNotes?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(trimmedExisting))
        {
            return trimmedIncoming;
        }

        if (string.IsNullOrEmpty(trimmedIncoming))
        {
            return trimmedExisting;
        }

        return string.Join(Environment.NewLine, new[] { trimmedExisting, trimmedIncoming });
    }

    private static ScheduleInterviewRequest NormalizeInterviewRequest(
        Guid candidateId,
        string stage,
        ScheduleInterviewRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new ScheduleInterviewRequest
        {
            CandidateId = request.CandidateId == Guid.Empty ? candidateId : request.CandidateId,
            VacancyId = request.VacancyId,
            ApplicationId = request.ApplicationId,
            StageId = request.StageId,
            ScheduledById = request.ScheduledById,
            Stage = string.IsNullOrWhiteSpace(request.Stage) ? stage : request.Stage,
            ScheduledAtUtc = request.ScheduledAtUtc,
            DurationMinutes = request.DurationMinutes,
            Mode = request.Mode,
            Location = request.Location,
            MeetingLink = request.MeetingLink,
            Interviewers = request.Interviewers ?? Array.Empty<string>(),
            Notes = request.Notes
        };
    }
}
