using System;
using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Request payload for enrolling an employee in a training course.
/// </summary>
public sealed class CreateCourseEnrollmentRequest
{
    [Required]
    public Guid CourseId { get; init; }

    [Required]
    public Guid EmployeeId { get; init; }

    public DateOnly? EnrolledOn { get; init; }
}
