using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for updating a payroll run.
/// </summary>
public sealed class UpdatePayrollRunRequest : IValidatableRequest
{
    [Required]
    public DateOnly PeriodStart { get; init; }

    [Required]
    public DateOnly PeriodEnd { get; init; }

    [MaxLength(50)]
    public string Status { get; init; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal TotalGrossPay { get; init; }

    [Range(0, double.MaxValue)]
    public decimal TotalNetPay { get; init; }

    [MaxLength(500)]
    public string Notes { get; init; } = string.Empty;

    public DateTime ProcessedAtUtc { get; init; }
}