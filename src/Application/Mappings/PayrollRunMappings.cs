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
            payrollRun.PeriodStart,
            payrollRun.PeriodEnd,
            payrollRun.ProcessedAtUtc,
            payrollRun.Status,
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
            PeriodStart = request.PeriodStart,
            PeriodEnd = request.PeriodEnd,
            ProcessedAtUtc = DateTime.UtcNow,
            Status = request.Status.Trim(),
            TotalGrossPay = request.TotalGrossPay,
            TotalNetPay = request.TotalNetPay,
            Notes = request.Notes.Trim()
        };
    }

    public static PayrollRun ApplyUpdates(this UpdatePayrollRunRequest request, PayrollRun existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        return new PayrollRun
        {
            Id = existing.Id,
            PeriodStart = request.PeriodStart,
            PeriodEnd = request.PeriodEnd,
            ProcessedAtUtc = request.ProcessedAtUtc,
            Status = request.Status.Trim(),
            TotalGrossPay = request.TotalGrossPay,
            TotalNetPay = request.TotalNetPay,
            Notes = request.Notes.Trim()
        };
    }
}
