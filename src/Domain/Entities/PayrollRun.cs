namespace HR.Domain.Entities;

/// <summary>
///     Represents a payroll cycle processing event.
/// </summary>
public sealed class PayrollRun
{
    public Guid Id { get; init; }

    public DateOnly PeriodStart { get; init; }

    public DateOnly PeriodEnd { get; init; }

    public DateTime ProcessedAtUtc { get; init; }

    public string Status { get; init; } = string.Empty;

    public decimal TotalGrossPay { get; init; }

    public decimal TotalNetPay { get; init; }

    public string Notes { get; init; } = string.Empty;
}
