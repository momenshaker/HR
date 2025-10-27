using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

/// <summary>
///     In-memory repository implementation for training courses.
/// </summary>
public sealed class InMemoryTrainingCourseRepository : ITrainingCourseRepository
{
    private readonly ConcurrentDictionary<Guid, TrainingCourse> _trainingCourses = new();

    public Task<IReadOnlyCollection<TrainingCourse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<TrainingCourse> snapshot = _trainingCourses.Values.ToList();
        return Task.FromResult(snapshot);
    }

    public Task<TrainingCourse?> GetByIdAsync(Guid trainingCourseId, CancellationToken cancellationToken = default)
    {
        _trainingCourses.TryGetValue(trainingCourseId, out var course);
        return Task.FromResult(course);
    }

    public Task<TrainingCourse> AddAsync(TrainingCourse trainingCourse, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(trainingCourse);

        if (!_trainingCourses.TryAdd(trainingCourse.Id, trainingCourse))
        {
            throw new InvalidOperationException($"A training course with id '{trainingCourse.Id}' already exists.");
        }

        return Task.FromResult(trainingCourse);
    }

    public Task<TrainingCourse?> UpdateAsync(TrainingCourse trainingCourse, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(trainingCourse);

        if (!_trainingCourses.ContainsKey(trainingCourse.Id))
        {
            return Task.FromResult<TrainingCourse?>(null);
        }

        _trainingCourses[trainingCourse.Id] = trainingCourse;
        return Task.FromResult<TrainingCourse?>(trainingCourse);
    }

    public Task<bool> RemoveAsync(Guid trainingCourseId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_trainingCourses.TryRemove(trainingCourseId, out _));
    }
}
