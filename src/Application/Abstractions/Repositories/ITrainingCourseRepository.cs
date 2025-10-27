using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository contract for managing <see cref="TrainingCourse" /> aggregates.
/// </summary>
public interface ITrainingCourseRepository
{
    Task<IReadOnlyCollection<TrainingCourse>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<TrainingCourse?> GetByIdAsync(Guid trainingCourseId, CancellationToken cancellationToken = default);

    Task<TrainingCourse> AddAsync(TrainingCourse trainingCourse, CancellationToken cancellationToken = default);

    Task<TrainingCourse?> UpdateAsync(TrainingCourse trainingCourse, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid trainingCourseId, CancellationToken cancellationToken = default);
}
