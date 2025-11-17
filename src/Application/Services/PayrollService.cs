using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;
using HR.Domain.Entities;

namespace HR.Application.Services;

public sealed record PayrollCalculationContext(
    IReadOnlyCollection<AttendanceRecord> AttendanceRecords,
    IReadOnlyCollection<LeaveRequest> LeaveRequests,
    IReadOnlyDictionary<Guid, LeaveType> LeaveTypes);

public interface IPayrollCalculator
{
    Task<IReadOnlyCollection<PayrollItem>> CalculateAsync(
        PayrollRun run,
        IReadOnlyCollection<Employee> employees,
        PayrollCalculationContext context,
        CancellationToken cancellationToken = default);
}

internal sealed record PayrollFormulaContext(decimal HourlyRate, decimal OvertimeHours, decimal UnpaidLeaveDays, decimal PeriodWorkDays);

public sealed class DefaultPayrollCalculator : IPayrollCalculator
{
    public Task<IReadOnlyCollection<PayrollItem>> CalculateAsync(
        PayrollRun run,
        IReadOnlyCollection<Employee> employees,
        PayrollCalculationContext context,
        CancellationToken cancellationToken = default)
    {
        var items = new List<PayrollItem>();

        foreach (var employee in employees.Where(e => e.IsActive))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var structure = NormalizeStructure(employee);
            var attendance = context.AttendanceRecords
                .Where(r => r.EmployeeId == employee.Id && r.WorkDate >= run.PeriodStart && r.WorkDate <= run.PeriodEnd)
                .ToArray();
            var leaveRequests = context.LeaveRequests
                .Where(l => l.EmployeeId == employee.Id && RangesOverlap(l.StartDate, l.EndDate, run.PeriodStart, run.PeriodEnd))
                .ToArray();

            var overtimeMinutes = attendance.Sum(a => a.OvertimeMinutes);
            var scheduledMinutes = attendance.Sum(a => a.ScheduledWorkMinutes);
            if (scheduledMinutes <= 0)
            {
                scheduledMinutes = 160 * 60; // fallback to standard 160-hour month
            }

            var baseSalary = structure.BasicSalary > 0 ? structure.BasicSalary : employee.BasicSalary;
            var hourlyRate = scheduledMinutes > 0 ? baseSalary / (scheduledMinutes / 60m) : baseSalary;
            var overtimeHours = overtimeMinutes / 60m;
            var unpaidLeaveDays = CalculateUnpaidLeaveDays(leaveRequests, context);
            var periodDays = Math.Max(1, run.PeriodEnd.DayNumber - run.PeriodStart.DayNumber + 1);
            var formulaContext = new PayrollFormulaContext(hourlyRate, overtimeHours, unpaidLeaveDays, periodDays);

            var earnings = new List<PayrollComponentAmount>
            {
                CreateComponent("BASE", "Basic Salary", PayrollComponentType.Earning, PayrollCalculationType.FixedAmount, baseSalary, true, true, null)
            };

            foreach (var component in structure.Earnings)
            {
                var amount = CalculateComponentAmount(component, baseSalary, earnings.Sum(e => e.Amount), formulaContext);
                if (amount != 0 || component.IsRecurring)
                {
                    earnings.Add(CreateComponentFromDefinition(component, amount));
                }
            }

            if (overtimeHours > 0)
            {
                var overtimeAmount = overtimeHours * hourlyRate * 1.5m;
                earnings.Add(CreateComponent("OT", "Overtime", PayrollComponentType.Earning, PayrollCalculationType.Formula, overtimeAmount, true, false, "Overtime"));
            }

            var deductions = new List<PayrollComponentAmount>();
            foreach (var component in structure.Deductions)
            {
                var amount = CalculateComponentAmount(component, baseSalary, earnings.Sum(e => e.Amount), formulaContext);
                if (amount != 0 || component.IsRecurring)
                {
                    deductions.Add(CreateComponentFromDefinition(component, amount));
                }
            }

            if (unpaidLeaveDays > 0)
            {
                var unpaidAmount = (baseSalary / periodDays) * unpaidLeaveDays;
                deductions.Add(CreateComponent("UNPAID", "Unpaid Leave", PayrollComponentType.Deduction, PayrollCalculationType.Formula, unpaidAmount, false, false, "UnpaidLeave"));
            }

            var gross = SafeRound(earnings.Sum(e => e.Amount));
            var deductionTotal = SafeRound(deductions.Sum(d => d.Amount));
            var net = SafeRound(gross - deductionTotal);
            var breakdown = new PayrollBreakdown { Earnings = earnings, Deductions = deductions };

            items.Add(new PayrollItem
            {
                Id = Guid.NewGuid(),
                RunId = run.Id,
                EmployeeId = employee.Id,
                Gross = gross,
                Deductions = deductionTotal,
                Net = net,
                Currency = "USD",
                Breakdown = breakdown.ToJson()
            });
        }

        return Task.FromResult<IReadOnlyCollection<PayrollItem>>(items);
    }

    private static SalaryStructure NormalizeStructure(Employee employee)
    {
        var structure = employee.SalaryStructure ?? SalaryStructure.Empty;
        return new SalaryStructure
        {
            BasicSalary = structure.BasicSalary > 0 ? structure.BasicSalary : employee.BasicSalary,
            PaySchedule = string.IsNullOrWhiteSpace(employee.PaySchedule) ? structure.PaySchedule : employee.PaySchedule,
            Earnings = structure.Earnings ?? Array.Empty<SalaryComponent>(),
            Deductions = structure.Deductions ?? Array.Empty<SalaryComponent>()
        };
    }

    private static decimal CalculateUnpaidLeaveDays(IEnumerable<LeaveRequest> leaveRequests, PayrollCalculationContext context)
    {
        decimal total = 0;
        foreach (var request in leaveRequests)
        {
            if (!string.Equals(request.Status, "Approved", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (request.LeaveTypeId != Guid.Empty && context.LeaveTypes.TryGetValue(request.LeaveTypeId, out var leaveType))
            {
                if (!leaveType.IsPaid)
                {
                    total += request.NumberOfDays;
                }
            }
            else if (string.Equals(request.LeaveType, "Unpaid", StringComparison.OrdinalIgnoreCase))
            {
                total += request.NumberOfDays;
            }
        }

        return total;
    }

    private static PayrollComponentAmount CreateComponent(string id, string name, PayrollComponentType type, PayrollCalculationType calcType, decimal amount, bool taxable, bool recurring, string? formula)
    {
        return new PayrollComponentAmount
        {
            ComponentId = id,
            Name = name,
            Type = type,
            CalculationType = calcType,
            Amount = SafeRound(amount),
            IsTaxable = taxable,
            IsRecurring = recurring,
            Formula = formula
        };
    }

    private static PayrollComponentAmount CreateComponentFromDefinition(SalaryComponent component, decimal amount)
    {
        return CreateComponent(
            component.Id.ToString(),
            component.Name,
            component.Type,
            component.CalculationType,
            amount,
            component.IsTaxable,
            component.IsRecurring,
            component.Formula);
    }

    private static decimal CalculateComponentAmount(SalaryComponent component, decimal baseSalary, decimal currentGross, PayrollFormulaContext formulaContext)
    {
        return component.CalculationType switch
        {
            PayrollCalculationType.FixedAmount => component.Value,
            PayrollCalculationType.PercentageOfBasic => baseSalary * component.Value / 100m,
            PayrollCalculationType.PercentageOfGross => currentGross * component.Value / 100m,
            PayrollCalculationType.Formula => EvaluateFormula(component, baseSalary, formulaContext),
            _ => 0m
        };
    }

    private static decimal EvaluateFormula(SalaryComponent component, decimal baseSalary, PayrollFormulaContext formulaContext)
    {
        return component.Formula?.ToLowerInvariant() switch
        {
            "overtime" => formulaContext.OvertimeHours * formulaContext.HourlyRate * (component.Value == 0 ? 1.5m : component.Value),
            "unpaidleave" => (baseSalary / formulaContext.PeriodWorkDays) * formulaContext.UnpaidLeaveDays,
            _ => 0m
        };
    }

    private static decimal SafeRound(decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }

    private static bool RangesOverlap(DateOnly aStart, DateOnly aEnd, DateOnly bStart, DateOnly bEnd)
    {
        return aStart <= bEnd && bStart <= aEnd;
    }
}

/// <inheritdoc />
public sealed class PayrollService : IPayrollService
{
    private readonly IPayrollRunRepository _runs;
    private readonly IPayrollItemRepository _items;
    private readonly IPayslipRepository _payslips;
    private readonly IEmployeeRepository _employees;
    private readonly IAttendanceRecordRepository _attendanceRecords;
    private readonly ILeaveRequestRepository _leaveRequests;
    private readonly ILeaveTypeRepository _leaveTypes;
    private readonly IPayrollCalculator _calculator;

    public PayrollService(
        IPayrollRunRepository runs,
        IPayrollItemRepository items,
        IPayslipRepository payslips,
        IEmployeeRepository employees,
        IAttendanceRecordRepository attendanceRecords,
        ILeaveRequestRepository leaveRequests,
        ILeaveTypeRepository leaveTypes,
        IPayrollCalculator? calculator = null)
    {
        _runs = runs;
        _items = items;
        _payslips = payslips;
        _employees = employees;
        _attendanceRecords = attendanceRecords;
        _leaveRequests = leaveRequests;
        _leaveTypes = leaveTypes;
        _calculator = calculator ?? new DefaultPayrollCalculator();
    }

    public async Task<PayrollRunDto> CreateRun(Guid organizationId, DateOnly periodStart, DateOnly periodEnd, DateOnly payDate, CancellationToken cancellationToken = default)
    {
        if (periodEnd < periodStart)
        {
            throw new ArgumentException("Period end must be on or after period start.");
        }

        if (payDate < periodEnd)
        {
            throw new ArgumentException("Pay date cannot precede the payroll period end.");
        }

        var existingRuns = await _runs.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var overlaps = existingRuns.Any(r => r.OrganizationId == organizationId && RangesOverlap(r.PeriodStart, r.PeriodEnd, periodStart, periodEnd));
        if (overlaps)
        {
            throw new InvalidOperationException("Payroll period overlaps an existing run.");
        }

        var entity = new CreatePayrollRunRequest
        {
            OrganizationId = organizationId,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            PayDate = payDate,
            Notes = string.Empty
        }.ToEntity();

        var created = await _runs.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        return created.ToDto();
    }

    public async Task<PayrollRunDto> Calculate(Guid runId, CancellationToken cancellationToken = default)
    {
        var run = await _runs.GetByIdAsync(runId, cancellationToken).ConfigureAwait(false)
                  ?? throw new KeyNotFoundException("Payroll run not found.");

        if (run.Status is "Locked" or "Paid" or "Approved")
        {
            throw new InvalidOperationException("Approved, locked, or paid runs cannot be recalculated.");
        }

        var employees = await _employees.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var attendance = await _attendanceRecords.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var leaveRequests = await _leaveRequests.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var leaveTypes = await _leaveTypes.GetAllAsync(cancellationToken).ConfigureAwait(false);

        var context = new PayrollCalculationContext(
            attendance.Where(a => a.WorkDate >= run.PeriodStart && a.WorkDate <= run.PeriodEnd).ToArray(),
            leaveRequests.Where(l => RangesOverlap(l.StartDate, l.EndDate, run.PeriodStart, run.PeriodEnd)).ToArray(),
            leaveTypes.ToDictionary(l => l.Id));

        var calculated = await _calculator.CalculateAsync(run, employees, context, cancellationToken).ConfigureAwait(false);

        await _items.RemoveByRunAsync(run.Id, cancellationToken).ConfigureAwait(false);
        await _items.AddRangeAsync(calculated, cancellationToken).ConfigureAwait(false);

        run.Status = "Calculated";
        run.TotalGrossPay = calculated.Sum(i => i.Gross);
        run.TotalNetPay = calculated.Sum(i => i.Net);

        var updated = await _runs.UpdateAsync(run, cancellationToken).ConfigureAwait(false) ?? run;
        return updated.ToDto();
    }

    public async Task<PayrollRunDto> MoveToReview(Guid runId, CancellationToken cancellationToken = default)
    {
        var run = await _runs.GetByIdAsync(runId, cancellationToken).ConfigureAwait(false)
                  ?? throw new KeyNotFoundException("Payroll run not found.");

        if (run.Status is not "Calculated")
        {
            throw new InvalidOperationException("Only calculated runs can move to under review.");
        }

        run.Status = "UnderReview";
        var updated = await _runs.UpdateAsync(run, cancellationToken).ConfigureAwait(false) ?? run;
        return updated.ToDto();
    }

    public async Task<PayrollRunDto> Approve(Guid runId, CancellationToken cancellationToken = default)
    {
        var run = await _runs.GetByIdAsync(runId, cancellationToken).ConfigureAwait(false)
                  ?? throw new KeyNotFoundException("Payroll run not found.");

        if (run.Status is not "UnderReview")
        {
            throw new InvalidOperationException("Only under-review runs can be approved.");
        }

        run.Status = "Approved";
        run.ApprovedAtUtc = DateTime.UtcNow;
        var updated = await _runs.UpdateAsync(run, cancellationToken).ConfigureAwait(false) ?? run;
        return updated.ToDto();
    }

    public async Task<PayrollRunDto> LockAsync(Guid runId, CancellationToken cancellationToken = default)
    {
        var run = await _runs.GetByIdAsync(runId, cancellationToken).ConfigureAwait(false)
                  ?? throw new KeyNotFoundException("Payroll run not found.");

        if (run.Status is not "Approved")
        {
            throw new InvalidOperationException("Only approved runs can be locked.");
        }

        run.Status = "Locked";
        var updated = await _runs.UpdateAsync(run, cancellationToken).ConfigureAwait(false) ?? run;
        return updated.ToDto();
    }

    public async Task<PayrollRunDto> MarkPaid(Guid runId, CancellationToken cancellationToken = default)
    {
        var run = await _runs.GetByIdAsync(runId, cancellationToken).ConfigureAwait(false)
                  ?? throw new KeyNotFoundException("Payroll run not found.");

        if (run.Status is not "Locked")
        {
            throw new InvalidOperationException("Only locked runs can be marked as paid.");
        }

        run.Status = "Paid";
        run.PaidAtUtc = DateTime.UtcNow;
        var updated = await _runs.UpdateAsync(run, cancellationToken).ConfigureAwait(false) ?? run;
        return updated.ToDto();
    }

    public async Task<IReadOnlyCollection<PayslipDto>> GeneratePayslips(Guid runId, CancellationToken cancellationToken = default)
    {
        var run = await _runs.GetByIdAsync(runId, cancellationToken).ConfigureAwait(false)
                  ?? throw new KeyNotFoundException("Payroll run not found.");

        if (run.Status is "Draft")
        {
            throw new InvalidOperationException("Run must be calculated before generating payslips.");
        }

        var existing = await _payslips.GetByRunAsync(runId, cancellationToken).ConfigureAwait(false);
        if (existing.Count > 0)
        {
            return existing.Select(x => x.ToDto()).ToArray();
        }

        var items = await _items.GetByRunAsync(runId, cancellationToken).ConfigureAwait(false);
        var now = DateTime.UtcNow;
        var slips = items
            .Select(i => new Payslip
            {
                Id = Guid.NewGuid(),
                RunId = runId,
                EmployeeId = i.EmployeeId,
                GeneratedAtUtc = now,
                PublicUrl = null
            })
            .ToArray();

        await _payslips.AddRangeAsync(slips, cancellationToken).ConfigureAwait(false);
        return slips.Select(s => s.ToDto()).ToArray();
    }

    public async Task<IReadOnlyCollection<PayrollRunDto>> GetRuns(Guid? organizationId, string? status, CancellationToken cancellationToken = default)
    {
        var runs = await _runs.GetAllAsync(cancellationToken).ConfigureAwait(false);
        if (organizationId is not null)
        {
            runs = runs.Where(r => r.OrganizationId == organizationId).ToArray();
        }
        if (!string.IsNullOrWhiteSpace(status))
        {
            runs = runs.Where(r => string.Equals(r.Status, status, StringComparison.OrdinalIgnoreCase)).ToArray();
        }
        return runs.Select(r => r.ToDto()).ToArray();
    }

    public async Task<PayrollRunDto?> GetRun(Guid id, CancellationToken cancellationToken = default)
    {
        var run = await _runs.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return run?.ToDto();
    }

    public async Task<IReadOnlyCollection<PayrollItemDto>> GetItems(Guid runId, CancellationToken cancellationToken = default)
    {
        var items = await _items.GetByRunAsync(runId, cancellationToken).ConfigureAwait(false);
        return items.Select(i => i.ToDto()).ToArray();
    }

    public async Task<IReadOnlyCollection<PayslipDto>> GetPayslips(Guid employeeId, DateOnly? periodStart, DateOnly? periodEnd, CancellationToken cancellationToken = default)
    {
        var slips = await _payslips.GetByEmployeeAsync(employeeId, periodStart, periodEnd, cancellationToken).ConfigureAwait(false);

        // For in-memory repo, apply date filters using runs
        if (periodStart is not null || periodEnd is not null)
        {
            var runs = await _runs.GetAllAsync(cancellationToken).ConfigureAwait(false);
            var filtered = slips.Where(s =>
            {
                var run = runs.FirstOrDefault(r => r.Id == s.RunId);
                if (run is null) return false;
                if (periodStart is not null && run.PeriodStart < periodStart) return false;
                if (periodEnd is not null && run.PeriodEnd > periodEnd) return false;
                return true;
            }).ToArray();

            return filtered.Select(s => s.ToDto()).ToArray();
        }

        return slips.Select(s => s.ToDto()).ToArray();
    }

    public async Task<IReadOnlyCollection<SalarySlipDto>> GetSalarySlipsAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var runs = await _runs.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var allItems = new List<PayrollItem>();
        foreach (var run in runs)
        {
            var runItems = await _items.GetByRunAsync(run.Id, cancellationToken).ConfigureAwait(false);
            allItems.AddRange(runItems.Where(i => i.EmployeeId == employeeId));
        }

        var slips = allItems.Select(i =>
        {
            var run = runs.First(r => r.Id == i.RunId);
            return new SalarySlipDto(
                i.RunId,
                employeeId,
                run.PeriodStart,
                run.PeriodEnd,
                run.CreatedAtUtc,
                run.Status,
                i.Gross,
                i.Net,
                run.Notes);
        }).ToArray();

        return slips;
    }

    private static bool RangesOverlap(DateOnly aStart, DateOnly aEnd, DateOnly bStart, DateOnly bEnd)
    {
        return aStart <= bEnd && bStart <= aEnd;
    }
}
