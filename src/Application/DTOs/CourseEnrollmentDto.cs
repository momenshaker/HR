using HR.Domain.Entities;
using System;

namespace HR.Application.DTOs;

/// <summary>
///     Read model describing an employee's enrollment in a training course.
/// </summary>
public sealed record CourseEnrollmentDto(
    Guid Id,
    Guid CourseId,
    Guid EmployeeId,
    DateOnly EnrolledOn,
    CourseEnrollmentStatus Status,
    decimal CompletionPercentage,
    DateOnly? CompletedOn,
    Guid? CertificationId);
