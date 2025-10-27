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
}
