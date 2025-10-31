using HR.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HR.Application.Abstractions.Repositories;

public interface ICourseSessionEnrollmentRepository
{
    Task<CourseSessionEnrollment?> GetAsync(Guid sessionId, Guid employeeId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CourseSessionEnrollment>> GetBySessionAsync(Guid sessionId, CancellationToken cancellationToken = default);

    Task<CourseSessionEnrollment> AddAsync(CourseSessionEnrollment enrollment, CancellationToken cancellationToken = default);

    Task<CourseSessionEnrollment> UpdateAsync(CourseSessionEnrollment enrollment, CancellationToken cancellationToken = default);
}

