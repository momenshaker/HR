using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository contract for managing <see cref="Candidate" /> aggregates.
/// </summary>
public interface ICandidateRepository
{
    Task<IReadOnlyCollection<Candidate>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Candidate?> GetByIdAsync(Guid candidateId, CancellationToken cancellationToken = default);

    Task<Candidate> AddAsync(Candidate candidate, CancellationToken cancellationToken = default);

    Task<Candidate?> UpdateAsync(Candidate candidate, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid candidateId, CancellationToken cancellationToken = default);
}
