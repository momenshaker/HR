using HR.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HR.Application.Abstractions.Repositories;

public interface ICourseSessionRepository
{
    Task<IReadOnlyCollection<CourseSession>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default);

    Task<CourseSession?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);

    Task<CourseSession> AddAsync(CourseSession session, CancellationToken cancellationToken = default);
}

