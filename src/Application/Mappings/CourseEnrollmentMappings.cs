using HR.Application.DTOs;
using HR.Domain.Entities;
using System;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="CourseEnrollment" /> entities.
/// </summary>
public static class CourseEnrollmentMappings
{
    public static CourseEnrollmentDto ToDto(this CourseEnrollment enrollment)
    {
        ArgumentNullException.ThrowIfNull(enrollment);

        return new CourseEnrollmentDto(
            enrollment.Id,
            enrollment.CourseId,
            enrollment.EmployeeId,
            enrollment.EnrolledOn,
            enrollment.Status,
            enrollment.CompletionPercentage,
            enrollment.CompletedOn,
            enrollment.CertificationId);
    }

    public static CourseEnrollment ToEntity(this CreateCourseEnrollmentRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var enrolledOn = request.EnrolledOn ?? DateOnly.FromDateTime(DateTime.UtcNow);

        return new CourseEnrollment
        {
            Id = Guid.NewGuid(),
            CourseId = request.CourseId,
            EmployeeId = request.EmployeeId,
            EnrolledOn = enrolledOn,
            Status = CourseEnrollmentStatus.Enrolled,
            CompletionPercentage = 0m
        };
    }

    public static CourseEnrollment ApplyProgressUpdate(
        this UpdateCourseEnrollmentProgressRequest request,
        CourseEnrollment existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        var normalizedCompletion = Math.Clamp(request.CompletionPercentage, 0m, 100m);
        var status = request.Status;

        if (normalizedCompletion >= 100m && status != CourseEnrollmentStatus.Withdrawn)
        {
            status = CourseEnrollmentStatus.Completed;
        }

        var completedOn = existing.CompletedOn;
        if (status == CourseEnrollmentStatus.Completed)
        {
            completedOn = request.CompletedOn ?? existing.CompletedOn ?? DateOnly.FromDateTime(DateTime.UtcNow);
        }
        else if (request.CompletedOn.HasValue)
        {
            completedOn = request.CompletedOn;
        }

        return new CourseEnrollment
        {
            Id = existing.Id,
            CourseId = existing.CourseId,
            EmployeeId = existing.EmployeeId,
            EnrolledOn = existing.EnrolledOn,
            Status = status,
            CompletionPercentage = normalizedCompletion,
            CompletedOn = completedOn,
            CertificationId = existing.CertificationId
        };
    }

    public static CourseEnrollment ApplyWithdrawal(this CourseEnrollment existing)
    {
        ArgumentNullException.ThrowIfNull(existing);

        return new CourseEnrollment
        {
            Id = existing.Id,
            CourseId = existing.CourseId,
            EmployeeId = existing.EmployeeId,
            EnrolledOn = existing.EnrolledOn,
            Status = CourseEnrollmentStatus.Withdrawn,
            CompletionPercentage = existing.CompletionPercentage,
            CompletedOn = existing.CompletedOn,
            CertificationId = existing.CertificationId
        };
    }

    public static CourseEnrollment Reactivate(this CourseEnrollment existing, DateOnly? enrolledOn = null)
    {
        ArgumentNullException.ThrowIfNull(existing);

        return new CourseEnrollment
        {
            Id = existing.Id,
            CourseId = existing.CourseId,
            EmployeeId = existing.EmployeeId,
            EnrolledOn = enrolledOn ?? DateOnly.FromDateTime(DateTime.UtcNow),
            Status = CourseEnrollmentStatus.Enrolled,
            CompletionPercentage = 0m,
            CompletedOn = null,
            CertificationId = null
        };
    }

    public static CourseEnrollment AttachCertification(this CourseEnrollment existing, Guid certificationId)
    {
        ArgumentNullException.ThrowIfNull(existing);

        return new CourseEnrollment
        {
            Id = existing.Id,
            CourseId = existing.CourseId,
            EmployeeId = existing.EmployeeId,
            EnrolledOn = existing.EnrolledOn,
            Status = existing.Status,
            CompletionPercentage = existing.CompletionPercentage,
            CompletedOn = existing.CompletedOn,
            CertificationId = certificationId
        };
    }
}
