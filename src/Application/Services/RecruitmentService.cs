using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class RecruitmentService : IRecruitmentService
{
    private readonly ICandidateRepository _candidateRepository;

    public RecruitmentService(ICandidateRepository candidateRepository)
    {
        _candidateRepository = candidateRepository;
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
}
