namespace HR.Application.DTOs;

public sealed record LeaveBalanceDto(
    Guid EmployeeId,
    Guid LeaveTypeId,
    int Year,
    decimal OpeningBalance,
    decimal Accrued,
    decimal Taken,
    decimal CarriedForward,
    decimal Reserved,
    decimal Remaining);

