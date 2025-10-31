namespace HR.Application.DTOs;

public sealed record PayrollItemDto(
    Guid Id,
    Guid RunId,
    Guid EmployeeId,
    decimal Gross,
    decimal Deductions,
    decimal Net,
    string Currency,
    string? Breakdown);

