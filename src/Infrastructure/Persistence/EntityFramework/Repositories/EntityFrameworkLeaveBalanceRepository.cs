using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkLeaveBalanceRepository : ILeaveBalanceRepository
{
    private readonly HrDbContext _db;
    public EntityFrameworkLeaveBalanceRepository(HrDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<LeaveBalance>> GetByEmployeeYearAsync(Guid employeeId, int year, CancellationToken cancellationToken = default)
    {
        return await _db.LeaveBalances.AsNoTracking()
            .Where(b => b.EmployeeId == employeeId && b.Year == year)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<LeaveBalance?> GetAsync(Guid employeeId, Guid leaveTypeId, int year, CancellationToken cancellationToken = default)
    {
        var entity = await _db.LeaveBalances.FindAsync(new object?[] { employeeId, leaveTypeId, year }, cancellationToken).ConfigureAwait(false);
        if (entity is not null)
        {
            _db.Entry(entity).State = EntityState.Detached;
        }
        return entity;
    }

    public async Task<LeaveBalance> UpsertAsync(LeaveBalance entity, CancellationToken cancellationToken = default)
    {
        var existing = await _db.LeaveBalances.FindAsync(new object?[] { entity.EmployeeId, entity.LeaveTypeId, entity.Year }, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            await _db.LeaveBalances.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            _db.Entry(existing).State = EntityState.Detached;
            _db.LeaveBalances.Update(entity);
        }

        await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        _db.Entry(entity).State = EntityState.Detached;
        return entity;
    }

    public async Task<bool> UpdateTakenAsync(Guid employeeId, Guid leaveTypeId, int year, decimal newTaken, byte[]? expectedRowVersion, CancellationToken cancellationToken = default)
    {
        var entity = await _db.LeaveBalances.FindAsync(new object?[] { employeeId, leaveTypeId, year }, cancellationToken).ConfigureAwait(false);
        if (entity is null) return false;

        if (expectedRowVersion is not null)
        {
            _db.Entry(entity).Property(e => e.RowVersion).OriginalValue = expectedRowVersion;
        }

        entity = new LeaveBalance
        {
            EmployeeId = entity.EmployeeId,
            LeaveTypeId = entity.LeaveTypeId,
            Year = entity.Year,
            Opening = entity.Opening,
            Accrued = entity.Accrued,
            Taken = newTaken,
            CarriedOver = entity.CarriedOver,
            RowVersion = entity.RowVersion
        };

        _db.LeaveBalances.Update(entity);
        try
        {
            await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            _db.Entry(entity).State = EntityState.Detached;
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
    }
}

