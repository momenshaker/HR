using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for creating a payroll run.
/// </summary>
public sealed class CreatePayrollRunRequest : IValidatableRequest
{
    [Required]
    public Guid OrganizationId { get; init; }

    [Required]
    public DateOnly PeriodStart { get; init; }

    [Required]
    public DateOnly PeriodEnd { get; init; }

    [MaxLength(500)]
    public string Notes { get; init; } = string.Empty;
}
