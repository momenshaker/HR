using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkReportingRelationshipRepository : EntityFrameworkRepository<ReportingRelationship>, IReportingRelationshipRepository
{
    public EntityFrameworkReportingRelationshipRepository(HrDbContext dbContext)
        : base(dbContext, relationship => relationship.Id)
    {
    }

    public async Task<IReadOnlyCollection<ReportingRelationship>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllInternalAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<ReportingRelationship?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<ReportingRelationship>> GetByManagerPositionAsync(
        Guid managerPositionId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Where(relationship => relationship.ManagerPositionId == managerPositionId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<ReportingRelationship>> GetByReportPositionAsync(
        Guid reportPositionId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Where(relationship => relationship.ReportPositionId == reportPositionId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public Task<ReportingRelationship> AddAsync(ReportingRelationship relationship, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(relationship, cancellationToken);
    }

    public Task<ReportingRelationship?> UpdateAsync(ReportingRelationship relationship, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(relationship, cancellationToken);
    }

    public Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(id, cancellationToken);
    }
}
