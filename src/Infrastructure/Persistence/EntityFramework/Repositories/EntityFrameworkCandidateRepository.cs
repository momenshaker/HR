using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkCandidateRepository : EntityFrameworkRepository<Candidate>, ICandidateRepository
{
    public EntityFrameworkCandidateRepository(HrDbContext dbContext)
        : base(dbContext, candidate => candidate.Id)
    {
    }

    public async Task<IReadOnlyCollection<Candidate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllInternalAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<Candidate?> GetByIdAsync(Guid candidateId, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(candidateId, cancellationToken);
    }

    public Task<Candidate> AddAsync(Candidate candidate, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(candidate, cancellationToken);
    }

    public Task<Candidate?> UpdateAsync(Candidate candidate, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(candidate, cancellationToken);
    }

    public Task<bool> RemoveAsync(Guid candidateId, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(candidateId, cancellationToken);
    }
}
