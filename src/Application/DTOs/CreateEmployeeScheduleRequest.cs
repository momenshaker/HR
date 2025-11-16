using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Payload to assign a work schedule to an employee.
/// </summary>
public sealed class CreateEmployeeScheduleRequest : IValidatableRequest
{
    [Required]
    public Guid EmployeeId { get; init; }

    [Required]
    public Guid WorkScheduleId { get; init; }

    [Required]
    public DateOnly EffectiveFrom { get; init; }

    public DateOnly? EffectiveTo { get; init; }
}
