namespace HR.Domain.Entities;

/// <summary>
///     Represents the lifecycle state of a course enrollment.
/// </summary>
public enum CourseEnrollmentStatus
{
    Enrolled = 0,
    InProgress = 1,
    Completed = 2,
    Withdrawn = 3
}
