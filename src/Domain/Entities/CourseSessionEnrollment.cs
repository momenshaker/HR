using System;

namespace HR.Domain.Entities;

public enum CourseSessionEnrollmentStatus
{
    Enrolled = 0,
    Completed = 1,
    Cancelled = 2
}

/// <summary>
///     Enrollment of an employee in a specific course session.
/// </summary>
public sealed class CourseSessionEnrollment
{
    public Guid SessionId { get; init; }

    public Guid EmployeeId { get; init; }

    public DateTime EnrolledAtUtc { get; init; }

    public CourseSessionEnrollmentStatus Status { get; init; }

    public decimal? Score { get; init; }

    public string? CertificateUrl { get; init; }
}

