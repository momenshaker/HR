namespace HR.Domain.Entities;

/// <summary>
///     Catalog entry describing a type of leave and its allowance rules.
/// </summary>
public sealed class LeaveType
{
    public Guid Id { get; init; }

    public string Code { get; init; } = string.Empty; // unique, <= 20

    public string Name { get; init; } = string.Empty;

    public bool RequiresApproval { get; init; }

    public decimal AnnualAllowanceDays { get; init; }

    public decimal CarryOverDays { get; init; }
}

