namespace HR.Application.DTOs;

/// <summary>
///     Read model representing a payroll run.
/// </summary>
public sealed record PayrollRunDto(
    Guid Id,
    DateOnly PeriodStart,
    DateOnly PeriodEnd,
    DateTime ProcessedAtUtc,
    string Status,
    decimal TotalGrossPay,
    decimal TotalNetPay,
    string Notes);
