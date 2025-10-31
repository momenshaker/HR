using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository contract for managing payslips.
/// </summary>
public interface IPayslipRepository
{
    Task<IReadOnlyCollection<Payslip>> GetByRunAsync(Guid runId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Payslip>> GetByEmployeeAsync(
        Guid employeeId,
        DateOnly? periodStart,
        DateOnly? periodEnd,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<Payslip> payslips, CancellationToken cancellationToken = default);
}

