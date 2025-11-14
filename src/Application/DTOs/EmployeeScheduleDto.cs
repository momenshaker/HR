namespace HR.Application.DTOs;

/// <summary>
///     Read model describing which schedule applies to an employee.
/// </summary>
public sealed record EmployeeScheduleDto(
    Guid Id,
    Guid EmployeeId,
    Guid WorkScheduleId,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo);
