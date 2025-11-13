namespace HR.Domain.Entities;

/// <summary>
///     Represents the policy that governs accrual and usage rules for a leave type within an organisation.
/// </summary>
public sealed class LeavePolicy
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public Guid LeaveTypeId { get; init; }

    public LeaveAccrualMethod AccrualMethod { get; init; }

    public decimal DaysPerYear { get; init; }

    public bool CarryForwardAllowed { get; init; }

    public decimal? MaxCarryForwardDays { get; init; }

    public bool IsNegativeBalanceAllowed { get; init; }
}

/// <summary>
///     Enumerates supported ways to accrue leave balances.
/// </summary>
public enum LeaveAccrualMethod
{
    Monthly = 0,
    Yearly = 1,
    Manual = 2
}
