using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

public sealed class InMemoryCourseSessionRepository : ICourseSessionRepository
{
    private readonly ConcurrentDictionary<Guid, CourseSession> _sessions = new();

    public Task<IReadOnlyCollection<CourseSession>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<CourseSession> snapshot = _sessions.Values.Where(s => s.CourseId == courseId).ToList();
        return Task.FromResult(snapshot);
    }

    public Task<CourseSession?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        _sessions.TryGetValue(sessionId, out var session);
        return Task.FromResult(session);
    }

    public Task<CourseSession> AddAsync(CourseSession session, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(session);
        if (!_sessions.TryAdd(session.Id, session))
        {
            throw new InvalidOperationException($"Course session with id '{session.Id}' already exists.");
        }
        return Task.FromResult(session);
    }
}

