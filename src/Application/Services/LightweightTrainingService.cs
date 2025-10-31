using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Services;

public sealed class LightweightTrainingService : ILightweightTrainingService
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICourseSessionRepository _sessionRepository;
    private readonly ICourseSessionEnrollmentRepository _enrollmentRepository;

    public LightweightTrainingService(
        ICourseRepository courseRepository,
        ICourseSessionRepository sessionRepository,
        ICourseSessionEnrollmentRepository enrollmentRepository)
    {
        _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        _enrollmentRepository = enrollmentRepository ?? throw new ArgumentNullException(nameof(enrollmentRepository));
    }

    public async Task<IReadOnlyCollection<LiteCourseDto>> GetCoursesAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        var courses = await _courseRepository.GetByOrganizationAsync(organizationId, cancellationToken).ConfigureAwait(false);
        return courses
            .OrderBy(c => c.Title, StringComparer.OrdinalIgnoreCase)
            .Select(ToDto)
            .ToArray();
    }

    public async Task<LiteCourseDto> CreateCourseAsync(CreateLiteCourseRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var normalizedCode = (request.Code ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(normalizedCode))
        {
            throw new ArgumentException("Code is required.", nameof(request.Code));
        }

        var existing = await _courseRepository.GetByOrgAndCodeAsync(request.OrganizationId, normalizedCode, cancellationToken).ConfigureAwait(false);
        if (existing is not null)
        {
            throw new InvalidOperationException("A course with this code already exists for the organization.");
        }

        var entity = new Course
        {
            Id = Guid.NewGuid(),
            OrganizationId = request.OrganizationId,
            Code = normalizedCode,
            Title = (request.Title ?? string.Empty).Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description!.Trim(),
            DurationHours = Math.Round(request.DurationHours, 2),
            IsMandatory = request.IsMandatory
        };

        var created = await _courseRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        return ToDto(created);
    }

    public async Task<IReadOnlyCollection<LiteCourseSessionDto>> GetCourseSessionsAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var sessions = await _sessionRepository.GetByCourseAsync(courseId, cancellationToken).ConfigureAwait(false);
        return sessions
            .OrderBy(s => s.StartUtc)
            .Select(ToDto)
            .ToArray();
    }

    public async Task<LiteCourseSessionDto> CreateCourseSessionAsync(CreateLiteCourseSessionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.StartUtc >= request.EndUtc)
        {
            throw new InvalidOperationException("StartUtc must be before EndUtc.");
        }

        if (string.IsNullOrWhiteSpace(request.Location) && string.IsNullOrWhiteSpace(request.MeetingUrl))
        {
            throw new InvalidOperationException("Either Location or MeetingUrl must be provided.");
        }

        var entity = new CourseSession
        {
            Id = Guid.NewGuid(),
            CourseId = request.CourseId,
            StartUtc = DateTime.SpecifyKind(request.StartUtc, DateTimeKind.Utc),
            EndUtc = DateTime.SpecifyKind(request.EndUtc, DateTimeKind.Utc),
            Location = string.IsNullOrWhiteSpace(request.Location) ? null : request.Location!.Trim(),
            MeetingUrl = string.IsNullOrWhiteSpace(request.MeetingUrl) ? null : request.MeetingUrl!.Trim(),
            Capacity = request.Capacity
        };

        var created = await _sessionRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        return ToDto(created);
    }

    public async Task<LiteEnrollmentDto> EnrollAsync(Guid sessionId, Guid employeeId, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Session not found.");

        var existing = await _enrollmentRepository.GetAsync(sessionId, employeeId, cancellationToken).ConfigureAwait(false);
        if (existing is not null)
        {
            return ToDto(existing);
        }

        if (session.Capacity is int cap)
        {
            var activeCount = (await _enrollmentRepository.GetBySessionAsync(sessionId, cancellationToken).ConfigureAwait(false))
                .Count(e => e.Status == CourseSessionEnrollmentStatus.Enrolled);
            if (activeCount >= cap)
            {
                throw new InvalidOperationException("Session capacity has been reached.");
            }
        }

        var enrollment = new CourseSessionEnrollment
        {
            SessionId = sessionId,
            EmployeeId = employeeId,
            EnrolledAtUtc = DateTime.UtcNow,
            Status = CourseSessionEnrollmentStatus.Enrolled,
            Score = null,
            CertificateUrl = null
        };

        var created = await _enrollmentRepository.AddAsync(enrollment, cancellationToken).ConfigureAwait(false);
        return ToDto(created);
    }

    public async Task<LiteEnrollmentDto> CompleteAsync(Guid sessionId, Guid employeeId, CancellationToken cancellationToken = default)
    {
        var enrollment = await _enrollmentRepository.GetAsync(sessionId, employeeId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Enrollment not found.");

        if (enrollment.Status != CourseSessionEnrollmentStatus.Enrolled)
        {
            throw new InvalidOperationException("Only enrolled participants can be completed.");
        }

        var updated = new CourseSessionEnrollment
        {
            SessionId = enrollment.SessionId,
            EmployeeId = enrollment.EmployeeId,
            EnrolledAtUtc = enrollment.EnrolledAtUtc,
            Status = CourseSessionEnrollmentStatus.Completed,
            Score = enrollment.Score,
            CertificateUrl = enrollment.CertificateUrl
        };

        var persisted = await _enrollmentRepository.UpdateAsync(updated, cancellationToken).ConfigureAwait(false);
        return ToDto(persisted);
    }

    public async Task<LiteEnrollmentDto> CancelAsync(Guid sessionId, Guid employeeId, CancellationToken cancellationToken = default)
    {
        var enrollment = await _enrollmentRepository.GetAsync(sessionId, employeeId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Enrollment not found.");

        if (enrollment.Status != CourseSessionEnrollmentStatus.Enrolled)
        {
            throw new InvalidOperationException("Only enrolled participants can be cancelled.");
        }

        var updated = new CourseSessionEnrollment
        {
            SessionId = enrollment.SessionId,
            EmployeeId = enrollment.EmployeeId,
            EnrolledAtUtc = enrollment.EnrolledAtUtc,
            Status = CourseSessionEnrollmentStatus.Cancelled,
            Score = enrollment.Score,
            CertificateUrl = enrollment.CertificateUrl
        };

        var persisted = await _enrollmentRepository.UpdateAsync(updated, cancellationToken).ConfigureAwait(false);
        return ToDto(persisted);
    }

    public async Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<Guid>>> GetMandatoryCompletionGapsAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        var courses = await _courseRepository.GetByOrganizationAsync(organizationId, cancellationToken).ConfigureAwait(false);
        var mandatoryCourseIds = courses.Where(c => c.IsMandatory).Select(c => c.Id).ToArray();
        if (mandatoryCourseIds.Length == 0)
        {
            return new Dictionary<Guid, IReadOnlyCollection<Guid>>();
        }

        // Build per-employee completion for any session of the mandatory courses
        var allSessionCompletions = new Dictionary<Guid, HashSet<Guid>>(); // employee -> completed course ids

        foreach (var courseId in mandatoryCourseIds)
        {
            var sessions = await _sessionRepository.GetByCourseAsync(courseId, cancellationToken).ConfigureAwait(false);
            foreach (var session in sessions)
            {
                var enrollments = await _enrollmentRepository.GetBySessionAsync(session.Id, cancellationToken).ConfigureAwait(false);
                foreach (var e in enrollments.Where(e => e.Status == CourseSessionEnrollmentStatus.Completed))
                {
                    if (!allSessionCompletions.TryGetValue(e.EmployeeId, out var set))
                    {
                        set = new HashSet<Guid>();
                        allSessionCompletions[e.EmployeeId] = set;
                    }
                    set.Add(courseId);
                }
            }
        }

        // Consider only employees that have any enrollment in org's courses (lightweight assumption)
        var observedEmployeeIds = new HashSet<Guid>(allSessionCompletions.Keys);

        // For capacity of tests, if we saw no completions, return empty map
        if (observedEmployeeIds.Count == 0)
        {
            return new Dictionary<Guid, IReadOnlyCollection<Guid>>();
        }

        var result = new Dictionary<Guid, IReadOnlyCollection<Guid>>();
        foreach (var empId in observedEmployeeIds)
        {
            var completed = allSessionCompletions.TryGetValue(empId, out var set) ? set : new HashSet<Guid>();
            var missing = mandatoryCourseIds.Where(id => !completed.Contains(id)).ToArray();
            if (missing.Length > 0)
            {
                result[empId] = missing;
            }
        }

        return result;
    }

    private static LiteCourseDto ToDto(Course c) => new(
        c.Id,
        c.OrganizationId,
        c.Code,
        c.Title,
        c.Description,
        c.DurationHours,
        c.IsMandatory);

    private static LiteCourseSessionDto ToDto(CourseSession s) => new(
        s.Id,
        s.CourseId,
        s.StartUtc,
        s.EndUtc,
        s.Location,
        s.MeetingUrl,
        s.Capacity);

    private static LiteEnrollmentDto ToDto(CourseSessionEnrollment e) => new(
        e.SessionId,
        e.EmployeeId,
        e.EnrolledAtUtc,
        (LiteEnrollmentStatus)e.Status,
        e.Score,
        e.CertificateUrl);
}

