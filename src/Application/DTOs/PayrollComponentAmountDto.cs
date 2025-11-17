using HR.Domain.Entities;

namespace HR.Application.DTOs;

/// <summary>
///     DTO representing a calculated payroll component line.
/// </summary>
public sealed record PayrollComponentAmountDto(
    string ComponentId,
    string Name,
    PayrollComponentType Type,
    PayrollCalculationType CalculationType,
    decimal Amount,
    bool IsTaxable,
    bool IsRecurring,
    string? Formula);
