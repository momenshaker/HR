namespace HR.Application.DTOs;

/// <summary>
///     Read model representing an employee-specific salary slip derived from a payroll run.
/// </summary>
public sealed record SalarySlipDto(
    Guid PayrollRunId,
    Guid EmployeeId,
    DateOnly PeriodStart,
    DateOnly PeriodEnd,
    DateTime ProcessedAtUtc,
    string Status,
    decimal GrossPay,
    decimal NetPay,
    string Notes);
