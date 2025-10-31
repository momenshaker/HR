namespace HR.Domain.Entities;

/// <summary>
///     Represents a payroll cycle processing event for an organization.
/// </summary>
public sealed class PayrollRun
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public DateOnly PeriodStart { get; init; }

    public DateOnly PeriodEnd { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime? ApprovedAtUtc { get; set; }

    public DateTime? PaidAtUtc { get; set; }

    /// <summary>
    ///     Run status: Draft|Calculated|Approved|Paid.
    /// </summary>
    public string Status { get; set; } = "Draft";

    public decimal TotalGrossPay { get; set; }

    public decimal TotalNetPay { get; set; }

    public string Notes { get; set; } = string.Empty;

    public byte[]? RowVersion { get; init; }

    public ICollection<PayrollItem> Items { get; set; } = new List<PayrollItem>();

    public ICollection<Payslip> Payslips { get; set; } = new List<Payslip>();
}
