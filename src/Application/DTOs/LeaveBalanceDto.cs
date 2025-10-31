namespace HR.Application.DTOs;

public sealed record LeaveBalanceDto(
    Guid EmployeeId,
    Guid LeaveTypeId,
    int Year,
    decimal Opening,
    decimal Accrued,
    decimal Taken,
    decimal CarriedOver,
    decimal Reserved,
    decimal Available);

