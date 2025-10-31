using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

public sealed class InMemoryPayslipRepository : IPayslipRepository
{
    private readonly ConcurrentDictionary<Guid, Payslip> _payslips = new();
    private readonly ConcurrentDictionary<Guid, PayrollRun> _runs = new();

    // Optional: allow registering runs for filtering; for in-memory, service can pass through runs
    public Task<IReadOnlyCollection<Payslip>> GetByRunAsync(Guid runId, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Payslip> snapshot = _payslips.Values.Where(p => p.RunId == runId).ToList();
        return Task.FromResult(snapshot);
    }

    public Task<IReadOnlyCollection<Payslip>> GetByEmployeeAsync(
        Guid employeeId,
        DateOnly? periodStart,
        DateOnly? periodEnd,
        CancellationToken cancellationToken = default)
    {
        var snapshot = _payslips.Values.Where(p => p.EmployeeId == employeeId).ToList();
        // In-memory does not know periods; service will post-filter if needed
        return Task.FromResult<IReadOnlyCollection<Payslip>>(snapshot);
    }

    public Task AddRangeAsync(IEnumerable<Payslip> payslips, CancellationToken cancellationToken = default)
    {
        foreach (var p in payslips)
        {
            _payslips[p.Id] = p;
        }
        return Task.CompletedTask;
    }
}

