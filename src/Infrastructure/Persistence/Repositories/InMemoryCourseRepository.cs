using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

public sealed class InMemoryCourseRepository : ICourseRepository
{
    private readonly ConcurrentDictionary<Guid, Course> _courses = new();

    public Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _courses.TryGetValue(id, out var course);
        return Task.FromResult(course);
    }

    public Task<Course?> GetByOrgAndCodeAsync(Guid organizationId, string code, CancellationToken cancellationToken = default)
    {
        var match = _courses.Values.FirstOrDefault(c => c.OrganizationId == organizationId && string.Equals(c.Code, code, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(match);
    }

    public Task<IReadOnlyCollection<Course>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Course> snapshot = _courses.Values.Where(c => c.OrganizationId == organizationId).ToList();
        return Task.FromResult(snapshot);
    }

    public Task<Course> AddAsync(Course course, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(course);
        if (!_courses.TryAdd(course.Id, course))
        {
            throw new InvalidOperationException($"Course with id '{course.Id}' already exists.");
        }
        return Task.FromResult(course);
    }
}

