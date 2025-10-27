using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

/// <summary>
///     In-memory repository implementation for payroll runs.
/// </summary>
public sealed class InMemoryPayrollRunRepository : IPayrollRunRepository
{
    private readonly ConcurrentDictionary<Guid, PayrollRun> _payrollRuns = new();

    public Task<IReadOnlyCollection<PayrollRun>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<PayrollRun> snapshot = _payrollRuns.Values.ToList();
        return Task.FromResult(snapshot);
    }

    public Task<PayrollRun?> GetByIdAsync(Guid payrollRunId, CancellationToken cancellationToken = default)
    {
        _payrollRuns.TryGetValue(payrollRunId, out var payrollRun);
        return Task.FromResult(payrollRun);
    }

    public Task<PayrollRun> AddAsync(PayrollRun payrollRun, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payrollRun);

        if (!_payrollRuns.TryAdd(payrollRun.Id, payrollRun))
        {
            throw new InvalidOperationException($"A payroll run with id '{payrollRun.Id}' already exists.");
        }

        return Task.FromResult(payrollRun);
    }

    public Task<PayrollRun?> UpdateAsync(PayrollRun payrollRun, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payrollRun);

        if (!_payrollRuns.ContainsKey(payrollRun.Id))
        {
            return Task.FromResult<PayrollRun?>(null);
        }

        _payrollRuns[payrollRun.Id] = payrollRun;
        return Task.FromResult<PayrollRun?>(payrollRun);
    }

    public Task<bool> RemoveAsync(Guid payrollRunId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_payrollRuns.TryRemove(payrollRunId, out _));
    }
}
