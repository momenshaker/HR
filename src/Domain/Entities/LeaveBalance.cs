namespace HR.Domain.Entities;

/// <summary>
///     Yearly leave balance for a specific employee and leave type.
///     Soft holds (reserved) are computed from pending requests and not stored here.
/// </summary>
public sealed class LeaveBalance
{
    public Guid EmployeeId { get; init; }

    public Guid LeaveTypeId { get; init; }

    public int Year { get; init; }

    public decimal Opening { get; init; }

    public decimal Accrued { get; init; }

    public decimal Taken { get; init; }

    public decimal CarriedOver { get; init; }

    public byte[]? RowVersion { get; init; }

    public decimal Remaining => Opening + Accrued + CarriedOver - Taken;
}

