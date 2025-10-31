namespace HR.Domain.Entities;

/// <summary>
///     Represents a generated payslip for an employee within a payroll run.
/// </summary>
public sealed class Payslip
{
    public Guid Id { get; init; }

    public Guid RunId { get; init; }

    public Guid EmployeeId { get; init; }

    public string? PublicUrl { get; set; }

    public DateTime GeneratedAtUtc { get; set; }

    public PayrollRun? Run { get; init; }
}

