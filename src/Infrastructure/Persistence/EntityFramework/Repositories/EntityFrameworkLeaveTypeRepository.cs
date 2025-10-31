using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkLeaveTypeRepository : ILeaveTypeRepository
{
    private readonly HrDbContext _db;
    public EntityFrameworkLeaveTypeRepository(HrDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<LeaveType>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _db.LeaveTypes.AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false);

    public async Task<LeaveType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _db.LeaveTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);

    public async Task<LeaveType> AddAsync(LeaveType entity, CancellationToken cancellationToken = default)
    {
        await _db.LeaveTypes.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        _db.Entry(entity).State = EntityState.Detached;
        return entity;
    }

    public async Task<LeaveType?> UpdateAsync(LeaveType entity, CancellationToken cancellationToken = default)
    {
        var existing = await _db.LeaveTypes.FindAsync(new object?[] { entity.Id }, cancellationToken).ConfigureAwait(false);
        if (existing is null) return null;
        _db.Entry(existing).State = EntityState.Detached;
        _db.LeaveTypes.Update(entity);
        await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        _db.Entry(entity).State = EntityState.Detached;
        return entity;
    }

    public async Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await _db.LeaveTypes.FindAsync(new object?[] { id }, cancellationToken).ConfigureAwait(false);
        if (existing is null) return false;
        _db.LeaveTypes.Remove(existing);
        await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }
}

