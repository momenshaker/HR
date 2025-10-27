using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service responsible for payroll management operations.
/// </summary>
public interface IPayrollService
{
    Task<IReadOnlyCollection<PayrollRunDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<PayrollRunDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PayrollRunDto> CreateAsync(CreatePayrollRunRequest request, CancellationToken cancellationToken = default);

    Task<PayrollRunDto?> UpdateAsync(Guid id, UpdatePayrollRunRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
