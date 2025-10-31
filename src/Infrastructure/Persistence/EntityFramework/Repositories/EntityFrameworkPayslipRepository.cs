using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkPayslipRepository : IPayslipRepository
{
    private readonly HrDbContext _dbContext;

    public EntityFrameworkPayslipRepository(HrDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Payslip>> GetByRunAsync(Guid runId, CancellationToken cancellationToken = default)
    {
        var slips = await _dbContext.Payslips
            .AsNoTracking()
            .Where(p => p.RunId == runId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return slips;
    }

    public async Task<IReadOnlyCollection<Payslip>> GetByEmployeeAsync(
        Guid employeeId,
        DateOnly? periodStart,
        DateOnly? periodEnd,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Payslips
            .AsNoTracking()
            .Where(p => p.EmployeeId == employeeId)
            .Join(
                _dbContext.PayrollRuns,
                p => p.RunId,
                r => r.Id,
                (p, r) => new { p, r })
            .AsQueryable();

        if (periodStart is not null)
        {
            query = query.Where(x => x.r.PeriodStart >= periodStart);
        }

        if (periodEnd is not null)
        {
            query = query.Where(x => x.r.PeriodEnd <= periodEnd);
        }

        var results = await query
            .Select(x => x.p)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return results;
    }

    public async Task AddRangeAsync(IEnumerable<Payslip> payslips, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payslips);
        await _dbContext.Payslips.AddRangeAsync(payslips, cancellationToken).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}

