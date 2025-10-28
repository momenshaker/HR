using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

/// <summary>
///     In-memory repository implementation for course enrollments.
/// </summary>
public sealed class InMemoryCourseEnrollmentRepository : ICourseEnrollmentRepository
{
    private readonly ConcurrentDictionary<Guid, CourseEnrollment> _enrollments = new();

    public Task<CourseEnrollment?> GetByIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        _enrollments.TryGetValue(enrollmentId, out var enrollment);
        return Task.FromResult(enrollment);
    }

    public Task<IReadOnlyCollection<CourseEnrollment>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<CourseEnrollment> snapshot = _enrollments.Values.Where(enrollment => enrollment.CourseId == courseId).ToList();
        return Task.FromResult(snapshot);
    }

    public Task<IReadOnlyCollection<CourseEnrollment>> GetByEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<CourseEnrollment> snapshot = _enrollments.Values.Where(enrollment => enrollment.EmployeeId == employeeId).ToList();
        return Task.FromResult(snapshot);
    }

    public Task<CourseEnrollment?> GetByCourseAndEmployeeAsync(Guid courseId, Guid employeeId, CancellationToken cancellationToken = default)
    {
        var enrollment = _enrollments.Values.FirstOrDefault(e => e.CourseId == courseId && e.EmployeeId == employeeId);
        return Task.FromResult(enrollment);
    }

    public Task<CourseEnrollment> AddAsync(CourseEnrollment enrollment, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(enrollment);

        if (!_enrollments.TryAdd(enrollment.Id, enrollment))
        {
            throw new InvalidOperationException($"An enrollment with id '{enrollment.Id}' already exists.");
        }

        return Task.FromResult(enrollment);
    }

    public Task<CourseEnrollment?> UpdateAsync(CourseEnrollment enrollment, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(enrollment);

        if (!_enrollments.ContainsKey(enrollment.Id))
        {
            return Task.FromResult<CourseEnrollment?>(null);
        }

        _enrollments[enrollment.Id] = enrollment;
        return Task.FromResult<CourseEnrollment?>(enrollment);
    }

    public Task<bool> RemoveAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_enrollments.TryRemove(enrollmentId, out _));
    }
}
