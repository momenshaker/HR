using HR.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository contract for managing course enrollment aggregates.
/// </summary>
public interface ICourseEnrollmentRepository
{
    Task<CourseEnrollment?> GetByIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CourseEnrollment>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CourseEnrollment>> GetByEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<CourseEnrollment?> GetByCourseAndEmployeeAsync(Guid courseId, Guid employeeId, CancellationToken cancellationToken = default);

    Task<CourseEnrollment> AddAsync(CourseEnrollment enrollment, CancellationToken cancellationToken = default);

    Task<CourseEnrollment?> UpdateAsync(CourseEnrollment enrollment, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid enrollmentId, CancellationToken cancellationToken = default);
}
