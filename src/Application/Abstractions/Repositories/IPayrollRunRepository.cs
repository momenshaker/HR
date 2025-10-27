using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository contract for managing <see cref="PayrollRun" /> aggregates.
/// </summary>
public interface IPayrollRunRepository
{
    Task<IReadOnlyCollection<PayrollRun>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PayrollRun?> GetByIdAsync(Guid payrollRunId, CancellationToken cancellationToken = default);

    Task<PayrollRun> AddAsync(PayrollRun payrollRun, CancellationToken cancellationToken = default);

    Task<PayrollRun?> UpdateAsync(PayrollRun payrollRun, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid payrollRunId, CancellationToken cancellationToken = default);
}
