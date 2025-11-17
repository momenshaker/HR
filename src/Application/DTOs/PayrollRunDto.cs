namespace HR.Application.DTOs;

/// <summary>
///     Read model representing a payroll run.
/// </summary>
public sealed record PayrollRunDto(
    Guid Id,
    Guid OrganizationId,
    DateOnly PeriodStart,
    DateOnly PeriodEnd,
    DateOnly PayDate,
    string Status,
    DateTime CreatedAtUtc,
    DateTime? ApprovedAtUtc,
    DateTime? PaidAtUtc,
    decimal TotalGrossPay,
    decimal TotalNetPay,
    string Notes);
