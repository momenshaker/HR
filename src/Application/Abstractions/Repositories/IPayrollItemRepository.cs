using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository contract for managing payroll line items.
/// </summary>
public interface IPayrollItemRepository
{
    Task<IReadOnlyCollection<PayrollItem>> GetByRunAsync(Guid runId, CancellationToken cancellationToken = default);

    Task<PayrollItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<PayrollItem> items, CancellationToken cancellationToken = default);

    Task RemoveByRunAsync(Guid runId, CancellationToken cancellationToken = default);
}

