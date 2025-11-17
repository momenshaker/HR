using System;
using System.Collections.Generic;

namespace HR.Application.DTOs;

public sealed record LeaveBalanceAdjustmentDto(Guid LeaveTypeId, decimal Remaining);

public sealed record SetLeaveBalancesRequest(
    Guid EmployeeId,
    int Year,
    IReadOnlyCollection<LeaveBalanceAdjustmentDto> Balances);
