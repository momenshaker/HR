using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

/// <summary>
///     In-memory repository implementation for recruitment candidates.
/// </summary>
public sealed class InMemoryCandidateRepository : ICandidateRepository
{
    private readonly ConcurrentDictionary<Guid, Candidate> _candidates = new();

    public Task<IReadOnlyCollection<Candidate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Candidate> snapshot = _candidates.Values.ToList();
        return Task.FromResult(snapshot);
    }

    public Task<Candidate?> GetByIdAsync(Guid candidateId, CancellationToken cancellationToken = default)
    {
        _candidates.TryGetValue(candidateId, out var candidate);
        return Task.FromResult(candidate);
    }

    public Task<Candidate> AddAsync(Candidate candidate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        if (!_candidates.TryAdd(candidate.Id, candidate))
        {
            throw new InvalidOperationException($"A candidate with id '{candidate.Id}' already exists.");
        }

        return Task.FromResult(candidate);
    }

    public Task<Candidate?> UpdateAsync(Candidate candidate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        if (!_candidates.ContainsKey(candidate.Id))
        {
            return Task.FromResult<Candidate?>(null);
        }

        _candidates[candidate.Id] = candidate;
        return Task.FromResult<Candidate?>(candidate);
    }

    public Task<bool> RemoveAsync(Guid candidateId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_candidates.TryRemove(candidateId, out _));
    }
}
