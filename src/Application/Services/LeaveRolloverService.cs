using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Domain.Entities;

namespace HR.Application.Services;

public sealed class LeaveRolloverService(
    IEmployeeRepository employees,
    ILeaveTypeRepository leaveTypes,
    ILeaveBalanceRepository balances) : ILeaveRolloverService
{
    public async Task RunAsync(int newYear, CancellationToken cancellationToken = default)
    {
        // For each employee and leave type, compute carry-over from previous year
        var emps = await employees.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var types = await leaveTypes.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var prevYear = newYear - 1;

        foreach (var e in emps)
        {
            foreach (var t in types)
            {
                var prev = await balances.GetAsync(e.Id, t.Id, prevYear, cancellationToken).ConfigureAwait(false)
                           ?? new LeaveBalance { EmployeeId = e.Id, LeaveTypeId = t.Id, Year = prevYear };

                var availablePrev = prev.Opening + prev.Accrued + prev.CarriedOver - prev.Taken;
                var carry = Math.Min(Math.Max(0m, availablePrev), t.CarryOverDays);

                var next = new LeaveBalance
                {
                    EmployeeId = e.Id,
                    LeaveTypeId = t.Id,
                    Year = newYear,
                    Opening = carry,
                    Accrued = t.AnnualAllowanceDays,
                    Taken = 0m,
                    CarriedOver = carry,
                    RowVersion = null
                };

                await balances.UpsertAsync(next, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}

