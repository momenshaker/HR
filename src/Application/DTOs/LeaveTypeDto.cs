namespace HR.Application.DTOs;

public sealed record LeaveTypeDto(
    Guid Id,
    string Code,
    string Name,
    bool IsPaid,
    bool RequiresApproval,
    bool RequiresAttachment,
    decimal AnnualAllowanceDays,
    decimal CarryOverDays,
    int? MaxConsecutiveDays,
    string Color);

