using System;

namespace HR.Domain.Entities;

/// <summary>
///     Represents an employee's enrollment in a specific training course.
/// </summary>
public sealed class CourseEnrollment
{
    public Guid Id { get; init; }

    public Guid CourseId { get; init; }

    public Guid EmployeeId { get; init; }

    public DateOnly EnrolledOn { get; init; }

    public CourseEnrollmentStatus Status { get; init; }

    public decimal CompletionPercentage { get; init; }

    public DateOnly? CompletedOn { get; init; }

    public Guid? CertificationId { get; init; }
}
