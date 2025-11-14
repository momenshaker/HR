using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Payload to update an employee schedule assignment.
/// </summary>
public sealed class UpdateEmployeeScheduleRequest : IValidatableRequest
{
    [Required]
    public Guid EmployeeId { get; init; }

    [Required]
    public Guid WorkScheduleId { get; init; }

    [Required]
    public DateOnly EffectiveFrom { get; init; }

    public DateOnly? EffectiveTo { get; init; }
}
