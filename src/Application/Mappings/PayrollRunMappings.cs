using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="PayrollRun" /> entities.
/// </summary>
public static class PayrollRunMappings
{
    public static PayrollRunDto ToDto(this PayrollRun payrollRun)
    {
        ArgumentNullException.ThrowIfNull(payrollRun);

        return new PayrollRunDto(
            payrollRun.Id,
            payrollRun.OrganizationId,
            payrollRun.PeriodStart,
            payrollRun.PeriodEnd,
            payrollRun.Status,
            payrollRun.CreatedAtUtc,
            payrollRun.ApprovedAtUtc,
            payrollRun.PaidAtUtc,
            payrollRun.TotalGrossPay,
            payrollRun.TotalNetPay,
            payrollRun.Notes);
    }

    public static PayrollRun ToEntity(this CreatePayrollRunRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new PayrollRun
        {
            Id = Guid.NewGuid(),
            OrganizationId = request.OrganizationId,
            PeriodStart = request.PeriodStart,
            PeriodEnd = request.PeriodEnd,
            CreatedAtUtc = DateTime.UtcNow,
            Status = "Draft",
            Notes = request.Notes.Trim(),
            TotalGrossPay = 0,
            TotalNetPay = 0
        };
    }

    public static PayrollItemDto ToDto(this PayrollItem item)
    {
        return new PayrollItemDto(
            item.Id,
            item.RunId,
            item.EmployeeId,
            item.Gross,
            item.Deductions,
            item.Net,
            item.Currency,
            item.Breakdown);
    }

    public static PayslipDto ToDto(this Payslip slip)
    {
        return new PayslipDto(
            slip.Id,
            slip.RunId,
            slip.EmployeeId,
            slip.PublicUrl,
            slip.GeneratedAtUtc);
    }
}
