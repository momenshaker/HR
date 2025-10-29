using System;
using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Request payload for enrolling an employee in a training course.
/// </summary>
public sealed class CreateCourseEnrollmentRequest : IValidatableRequest
{
    [Required]
    public Guid CourseId { get; init; }

    [Required]
    public Guid EmployeeId { get; init; }

    public DateOnly? EnrolledOn { get; init; }
}