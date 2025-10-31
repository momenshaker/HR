namespace HR.Domain.Entities;

/// <summary>
///     Represents an employee-specific line item within a payroll run.
/// </summary>
public sealed class PayrollItem
{
    public Guid Id { get; init; }

    public Guid RunId { get; init; }

    public Guid EmployeeId { get; init; }

    public decimal Gross { get; set; }

    public decimal Deductions { get; set; }

    public decimal Net { get; set; }

    public string Currency { get; set; } = "USD";

    /// <summary>
    ///     JSON payload with itemized earnings/deductions breakdown.
    /// </summary>
    public string? Breakdown { get; set; }

    public byte[]? RowVersion { get; init; }

    public PayrollRun? Run { get; init; }
}

