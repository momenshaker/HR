using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service responsible for payroll management operations.
/// </summary>
public interface IPayrollService
{
    // New API per spec
    Task<PayrollRunDto> CreateRun(Guid organizationId, DateOnly periodStart, DateOnly periodEnd, CancellationToken cancellationToken = default);

    Task<PayrollRunDto> Calculate(Guid runId, CancellationToken cancellationToken = default);

    Task<PayrollRunDto> Approve(Guid runId, CancellationToken cancellationToken = default);

    Task<PayrollRunDto> MarkPaid(Guid runId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PayslipDto>> GeneratePayslips(Guid runId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PayrollRunDto>> GetRuns(Guid? organizationId, string? status, CancellationToken cancellationToken = default);

    Task<PayrollRunDto?> GetRun(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PayrollItemDto>> GetItems(Guid runId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PayslipDto>> GetPayslips(Guid employeeId, DateOnly? periodStart, DateOnly? periodEnd, CancellationToken cancellationToken = default);

    // Back-compat for employee self-service
    Task<IReadOnlyCollection<SalarySlipDto>> GetSalarySlipsAsync(Guid employeeId, CancellationToken cancellationToken = default);
}
