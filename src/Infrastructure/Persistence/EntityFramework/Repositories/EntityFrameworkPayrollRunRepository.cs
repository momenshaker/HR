using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkPayrollRunRepository : EntityFrameworkRepository<PayrollRun>, IPayrollRunRepository
{
    public EntityFrameworkPayrollRunRepository(HrDbContext dbContext)
        : base(dbContext, payroll => payroll.Id)
    {
    }

    public async Task<IReadOnlyCollection<PayrollRun>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllInternalAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<PayrollRun?> GetByIdAsync(Guid payrollRunId, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(payrollRunId, cancellationToken);
    }

    public Task<PayrollRun> AddAsync(PayrollRun payrollRun, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(payrollRun, cancellationToken);
    }

    public Task<PayrollRun?> UpdateAsync(PayrollRun payrollRun, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(payrollRun, cancellationToken);
    }

    public Task<bool> RemoveAsync(Guid payrollRunId, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(payrollRunId, cancellationToken);
    }
}
