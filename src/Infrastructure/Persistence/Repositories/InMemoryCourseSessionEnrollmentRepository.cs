using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

public sealed class InMemoryCourseSessionEnrollmentRepository : ICourseSessionEnrollmentRepository
{
    private readonly ConcurrentDictionary<string, CourseSessionEnrollment> _enrollments = new();

    private static string Key(Guid sessionId, Guid employeeId) => $"{sessionId:N}:{employeeId:N}";

    public Task<CourseSessionEnrollment?> GetAsync(Guid sessionId, Guid employeeId, CancellationToken cancellationToken = default)
    {
        _enrollments.TryGetValue(Key(sessionId, employeeId), out var enrollment);
        return Task.FromResult(enrollment);
    }

    public Task<IReadOnlyCollection<CourseSessionEnrollment>> GetBySessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<CourseSessionEnrollment> snapshot = _enrollments.Values.Where(e => e.SessionId == sessionId).ToList();
        return Task.FromResult(snapshot);
    }

    public Task<CourseSessionEnrollment> AddAsync(CourseSessionEnrollment enrollment, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(enrollment);
        if (!_enrollments.TryAdd(Key(enrollment.SessionId, enrollment.EmployeeId), enrollment))
        {
            throw new InvalidOperationException("Enrollment already exists for this session and employee.");
        }
        return Task.FromResult(enrollment);
    }

    public Task<CourseSessionEnrollment> UpdateAsync(CourseSessionEnrollment enrollment, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(enrollment);
        _enrollments[Key(enrollment.SessionId, enrollment.EmployeeId)] = enrollment;
        return Task.FromResult(enrollment);
    }
}

