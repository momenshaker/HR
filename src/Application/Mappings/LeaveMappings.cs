using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

public static class LeaveMappings
{
    public static LeaveTypeDto ToDto(this LeaveType entity)
    {
        return new LeaveTypeDto(
            entity.Id,
            entity.Code,
            entity.Name,
            entity.RequiresApproval,
            entity.AnnualAllowanceDays,
            entity.CarryOverDays);
    }

    public static LeaveBalanceDto ToDto(this LeaveBalance entity, decimal reserved)
    {
        var currentAvailable = entity.Opening + entity.Accrued + entity.CarriedOver - entity.Taken;
        return new LeaveBalanceDto(
            entity.EmployeeId,
            entity.LeaveTypeId,
            entity.Year,
            entity.Opening,
            entity.Accrued,
            entity.Taken,
            entity.CarriedOver,
            reserved,
            currentAvailable - reserved);
    }
}

