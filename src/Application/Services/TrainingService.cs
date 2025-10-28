using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;
using HR.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class TrainingService : ITrainingService
{
    private readonly ITrainingCourseRepository _courseRepository;
    private readonly ICourseEnrollmentRepository _enrollmentRepository;
    private readonly ICourseCertificationRepository _certificationRepository;

    public TrainingService(
        ITrainingCourseRepository courseRepository,
        ICourseEnrollmentRepository enrollmentRepository,
        ICourseCertificationRepository certificationRepository)
    {
        _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
        _enrollmentRepository = enrollmentRepository ?? throw new ArgumentNullException(nameof(enrollmentRepository));
        _certificationRepository = certificationRepository ?? throw new ArgumentNullException(nameof(certificationRepository));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<TrainingCourseDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var courses = await _courseRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return courses.Select(course => course.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<TrainingCourseDto>> GetTrainingCoursesAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        var enrollments = await _enrollmentRepository.GetByEmployeeAsync(employeeId, cancellationToken)
            .ConfigureAwait(false);

        if (enrollments.Count == 0)
        {
            return Array.Empty<TrainingCourseDto>();
        }

        var enrolledCourseIds = enrollments
            .Where(enrollment => enrollment.Status != CourseEnrollmentStatus.Withdrawn)
            .Select(enrollment => enrollment.CourseId)
            .Distinct()
            .ToHashSet();

        if (enrolledCourseIds.Count == 0)
        {
            return Array.Empty<TrainingCourseDto>();
        }

        var courses = await _courseRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);

        return courses
            .Where(course => enrolledCourseIds.Contains(course.Id))
            .Select(course => course.ToDto())
            .OrderBy(course => course.Title, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<TrainingCourseDto>> GetByCompetencyAsync(string competencyCode, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(competencyCode);

        var normalized = competencyCode.Trim();
        var courses = await _courseRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);

        return courses
            .Where(course => course.CompetencyCodes.Any(code => string.Equals(code, normalized, StringComparison.OrdinalIgnoreCase)))
            .Select(course => course.ToDto())
            .ToArray();
    }

    /// <inheritdoc />
    public async Task<TrainingCourseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await _courseRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return course?.ToDto();
    }

    /// <inheritdoc />
    public async Task<TrainingCourseDto> CreateAsync(CreateTrainingCourseRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = request.ToEntity();
        var created = await _courseRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<TrainingCourseDto?> UpdateAsync(Guid id, UpdateTrainingCourseRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _courseRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updatedEntity = request.ApplyUpdates(existing);
        var persisted = await _courseRepository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);

        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _courseRepository.RemoveAsync(id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CourseEnrollmentDto> EnrollEmployeeAsync(CreateCourseEnrollmentRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken).ConfigureAwait(false);
        if (course is null)
        {
            throw new InvalidOperationException($"Training course '{request.CourseId}' was not found.");
        }

        var existingEnrollment = await _enrollmentRepository
            .GetByCourseAndEmployeeAsync(request.CourseId, request.EmployeeId, cancellationToken)
            .ConfigureAwait(false);

        if (existingEnrollment is not null)
        {
            if (existingEnrollment.Status != CourseEnrollmentStatus.Withdrawn)
            {
                throw new InvalidOperationException("The employee is already enrolled in this course.");
            }

            var reactivated = existingEnrollment.Reactivate(request.EnrolledOn);
            var updated = await _enrollmentRepository.UpdateAsync(reactivated, cancellationToken).ConfigureAwait(false);
            return updated!.ToDto();
        }

        var entity = request.ToEntity();
        var created = await _enrollmentRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<CourseEnrollmentDto?> UpdateEnrollmentProgressAsync(Guid enrollmentId, UpdateCourseEnrollmentProgressRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _enrollmentRepository.GetByIdAsync(enrollmentId, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updated = request.ApplyProgressUpdate(existing);
        var persisted = await _enrollmentRepository.UpdateAsync(updated, cancellationToken).ConfigureAwait(false);

        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public async Task<CourseEnrollmentDto?> WithdrawEnrollmentAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        var existing = await _enrollmentRepository.GetByIdAsync(enrollmentId, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updated = existing.ApplyWithdrawal();
        var persisted = await _enrollmentRepository.UpdateAsync(updated, cancellationToken).ConfigureAwait(false);

        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<CourseEnrollmentDto>> GetCourseEnrollmentsAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var enrollments = await _enrollmentRepository.GetByCourseAsync(courseId, cancellationToken).ConfigureAwait(false);
        return enrollments.Select(enrollment => enrollment.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<CourseEnrollmentDto>> GetEmployeeEnrollmentsAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var enrollments = await _enrollmentRepository.GetByEmployeeAsync(employeeId, cancellationToken).ConfigureAwait(false);
        return enrollments.Select(enrollment => enrollment.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<CourseProgressAnalyticsDto> GetCourseProgressAnalyticsAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var enrollments = await _enrollmentRepository.GetByCourseAsync(courseId, cancellationToken).ConfigureAwait(false);

        var total = enrollments.Count;
        var active = enrollments.Count(enrollment => enrollment.Status is CourseEnrollmentStatus.Enrolled or CourseEnrollmentStatus.InProgress);
        var completed = enrollments.Count(enrollment => enrollment.Status == CourseEnrollmentStatus.Completed);
        var averageCompletion = total == 0 ? 0m : Math.Round(enrollments.Average(enrollment => enrollment.CompletionPercentage), 2);

        return new CourseProgressAnalyticsDto(
            courseId,
            total,
            active,
            completed,
            averageCompletion,
            DateTime.UtcNow);
    }

    /// <inheritdoc />
    public async Task<CourseCertificationDto> IssueCertificationAsync(IssueCourseCertificationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken).ConfigureAwait(false);
        if (course is null)
        {
            throw new InvalidOperationException($"Training course '{request.CourseId}' was not found.");
        }

        if (!course.OffersCertification)
        {
            throw new InvalidOperationException("The selected course does not issue certifications.");
        }

        var enrollment = await _enrollmentRepository
            .GetByCourseAndEmployeeAsync(request.CourseId, request.EmployeeId, cancellationToken)
            .ConfigureAwait(false);

        if (enrollment is null)
        {
            throw new InvalidOperationException("The employee must be enrolled in the course before certification can be issued.");
        }

        if (enrollment.Status != CourseEnrollmentStatus.Completed || enrollment.CompletionPercentage < 100m)
        {
            throw new InvalidOperationException("Certification can only be issued for completed enrollments.");
        }

        var normalizedCertificate = request.CertificateNumber.Trim().ToUpperInvariant();
        var existingCertificate = await _certificationRepository
            .GetByCertificateNumberAsync(normalizedCertificate, cancellationToken)
            .ConfigureAwait(false);

        if (existingCertificate is not null)
        {
            throw new InvalidOperationException("The certificate number is already in use.");
        }

        var certificationEntity = request.ToEntity();
        var createdCertification = await _certificationRepository.AddAsync(certificationEntity, cancellationToken).ConfigureAwait(false);

        var enrollmentWithCertification = enrollment.AttachCertification(createdCertification.Id);
        await _enrollmentRepository.UpdateAsync(enrollmentWithCertification, cancellationToken).ConfigureAwait(false);

        return createdCertification.ToDto();
    }

    /// <inheritdoc />
    public async Task<CourseCertificationDto?> RevokeCertificationAsync(Guid certificationId, RevokeCourseCertificationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _certificationRepository.GetByIdAsync(certificationId, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updated = existing.ApplyGovernanceUpdate(CertificationStatus.Revoked, request.GovernanceNotes);
        var persisted = await _certificationRepository.UpdateAsync(updated, cancellationToken).ConfigureAwait(false);

        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<CourseCertificationDto>> GetCourseCertificationsAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var certifications = await _certificationRepository.GetByCourseAsync(courseId, cancellationToken).ConfigureAwait(false);
        return certifications.Select(certification => certification.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<CourseCertificationDto>> GetEmployeeCertificationsAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var certifications = await _certificationRepository.GetByEmployeeAsync(employeeId, cancellationToken).ConfigureAwait(false);
        return certifications.Select(certification => certification.ToDto()).ToArray();
    }
}
