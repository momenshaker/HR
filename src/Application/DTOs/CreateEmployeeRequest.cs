using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for creating an employee.
/// </summary>
public sealed class CreateEmployeeRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public Guid DepartmentId { get; init; }

    [Required]
    public DateOnly EmploymentStartDate { get; init; }

    [MaxLength(150)]
    public string JobTitle { get; init; } = string.Empty;

    public DateOnly? EmploymentEndDate { get; init; }

    public DateOnly? DateOfBirth { get; init; }
}
