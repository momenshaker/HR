namespace HR.Application.DTOs;

public sealed record LeaveTypeDto(
    Guid Id,
    string Code,
    string Name,
    bool RequiresApproval,
    decimal AnnualAllowanceDays,
    decimal CarryOverDays);

