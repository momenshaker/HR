using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;
using HR.Domain.Entities;

namespace HR.Application.Services;

public interface IPayrollCalculator
{
    Task<IReadOnlyCollection<PayrollItem>> CalculateAsync(
        PayrollRun run,
        IReadOnlyCollection<Employee> employees,
        CancellationToken cancellationToken = default);
}

public sealed class DefaultPayrollCalculator : IPayrollCalculator
{
    public Task<IReadOnlyCollection<PayrollItem>> CalculateAsync(
        PayrollRun run,
        IReadOnlyCollection<Employee> employees,
        CancellationToken cancellationToken = default)
    {
        // Stub deterministic calculator: creates zeroed items for each employee
        var items = employees
            .Select(e => new PayrollItem
            {
                Id = Guid.NewGuid(),
                RunId = run.Id,
                EmployeeId = e.Id,
                Gross = 0m,
                Deductions = 0m,
                Net = 0m,
                Currency = "USD",
                Breakdown = "{}"
            })
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<PayrollItem>>(items);
    }
}

/// <inheritdoc />
public sealed class PayrollService : IPayrollService
{
    private readonly IPayrollRunRepository _runs;
    private readonly IPayrollItemRepository _items;
    private readonly IPayslipRepository _payslips;
    private readonly IEmployeeRepository _employees;
    private readonly IPayrollCalculator _calculator;

    public PayrollService(
        IPayrollRunRepository runs,
        IPayrollItemRepository items,
        IPayslipRepository payslips,
        IEmployeeRepository employees,
        IPayrollCalculator? calculator = null)
    {
        _runs = runs;
        _items = items;
        _payslips = payslips;
        _employees = employees;
        _calculator = calculator ?? new DefaultPayrollCalculator();
    }

    public async Task<PayrollRunDto> CreateRun(Guid organizationId, DateOnly periodStart, DateOnly periodEnd, CancellationToken cancellationToken = default)
    {
        if (periodEnd < periodStart)
        {
            throw new ArgumentException("Period end must be on or after period start.");
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
            Notes = string.Empty
        }.ToEntity();

        var created = await _runs.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        return created.ToDto();
    }

    public async Task<PayrollRunDto> Calculate(Guid runId, CancellationToken cancellationToken = default)
    {
        var run = await _runs.GetByIdAsync(runId, cancellationToken).ConfigureAwait(false)
                  ?? throw new KeyNotFoundException("Payroll run not found.");

        if (run.Status is "Approved" or "Paid")
        {
            return run.ToDto();
        }

        var existingItems = await _items.GetByRunAsync(run.Id, cancellationToken).ConfigureAwait(false);
        if (existingItems.Count == 0)
        {
            var employees = await _employees.GetAllAsync(cancellationToken).ConfigureAwait(false);
            var calculated = await _calculator.CalculateAsync(run, employees, cancellationToken).ConfigureAwait(false);
            await _items.AddRangeAsync(calculated, cancellationToken).ConfigureAwait(false);
            existingItems = calculated;
        }

        run.Status = "Calculated";
        run.TotalGrossPay = existingItems.Sum(i => i.Gross);
        run.TotalNetPay = existingItems.Sum(i => i.Net);

        var updated = await _runs.UpdateAsync(run, cancellationToken).ConfigureAwait(false) ?? run;
        return updated.ToDto();
    }

    public async Task<PayrollRunDto> Approve(Guid runId, CancellationToken cancellationToken = default)
    {
        var run = await _runs.GetByIdAsync(runId, cancellationToken).ConfigureAwait(false)
                  ?? throw new KeyNotFoundException("Payroll run not found.");

        if (run.Status is not "Calculated")
        {
            throw new InvalidOperationException("Only calculated runs can be approved.");
        }

        run.Status = "Approved";
        run.ApprovedAtUtc = DateTime.UtcNow;
        var updated = await _runs.UpdateAsync(run, cancellationToken).ConfigureAwait(false) ?? run;
        return updated.ToDto();
    }

    public async Task<PayrollRunDto> MarkPaid(Guid runId, CancellationToken cancellationToken = default)
    {
        var run = await _runs.GetByIdAsync(runId, cancellationToken).ConfigureAwait(false)
                  ?? throw new KeyNotFoundException("Payroll run not found.");

        if (run.Status is not "Approved")
        {
            throw new InvalidOperationException("Only approved runs can be marked as paid.");
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
