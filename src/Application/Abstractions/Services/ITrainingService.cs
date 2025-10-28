using HR.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service responsible for training and development operations.
/// </summary>
public interface ITrainingService
{
    Task<IReadOnlyCollection<TrainingCourseDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<TrainingCourseDto>> GetTrainingCoursesAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<TrainingCourseDto>> GetByCompetencyAsync(string competencyCode, CancellationToken cancellationToken = default);

    Task<TrainingCourseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TrainingCourseDto> CreateAsync(CreateTrainingCourseRequest request, CancellationToken cancellationToken = default);

    Task<TrainingCourseDto?> UpdateAsync(Guid id, UpdateTrainingCourseRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CourseEnrollmentDto> EnrollEmployeeAsync(CreateCourseEnrollmentRequest request, CancellationToken cancellationToken = default);

    Task<CourseEnrollmentDto?> UpdateEnrollmentProgressAsync(Guid enrollmentId, UpdateCourseEnrollmentProgressRequest request, CancellationToken cancellationToken = default);

    Task<CourseEnrollmentDto?> WithdrawEnrollmentAsync(Guid enrollmentId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CourseEnrollmentDto>> GetCourseEnrollmentsAsync(Guid courseId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CourseEnrollmentDto>> GetEmployeeEnrollmentsAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<CourseProgressAnalyticsDto> GetCourseProgressAnalyticsAsync(Guid courseId, CancellationToken cancellationToken = default);

    Task<CourseCertificationDto> IssueCertificationAsync(IssueCourseCertificationRequest request, CancellationToken cancellationToken = default);

    Task<CourseCertificationDto?> RevokeCertificationAsync(Guid certificationId, RevokeCourseCertificationRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CourseCertificationDto>> GetCourseCertificationsAsync(Guid courseId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CourseCertificationDto>> GetEmployeeCertificationsAsync(Guid employeeId, CancellationToken cancellationToken = default);
}
