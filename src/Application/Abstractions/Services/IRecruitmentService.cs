using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service responsible for recruitment and ATS operations.
/// </summary>
public interface IRecruitmentService
{
    Task<IReadOnlyCollection<CandidateDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<CandidateDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CandidateDto> CreateAsync(CreateCandidateRequest request, CancellationToken cancellationToken = default);

    Task<CandidateDto?> UpdateAsync(Guid id, UpdateCandidateRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CandidateDto?> AdvanceCandidateAsync(Guid id, AdvanceCandidateRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<VacancyDto>> GetVacanciesAsync(CancellationToken cancellationToken = default);

    Task<VacancyDto?> GetVacancyByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<VacancyDto> PublishVacancyAsync(CreateVacancyRequest request, CancellationToken cancellationToken = default);

    Task<VacancyDto?> UpdateVacancyAsync(Guid id, UpdateVacancyRequest request, CancellationToken cancellationToken = default);

    Task<bool> CloseVacancyAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<InterviewScheduleDto>> GetInterviewsAsync(
        Guid? vacancyId = null,
        Guid? candidateId = null,
        bool onlyUpcoming = false,
        CancellationToken cancellationToken = default);

    Task<InterviewScheduleDto> ScheduleInterviewAsync(ScheduleInterviewRequest request, CancellationToken cancellationToken = default);

    Task<InterviewScheduleDto?> UpdateInterviewAsync(
        Guid id,
        UpdateInterviewScheduleRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> CancelInterviewAsync(Guid id, CancellationToken cancellationToken = default);

    Task<RecruitmentInsightsDto> GetInsightsAsync(CancellationToken cancellationToken = default);
}
