using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkTrainingCourseRepository : EntityFrameworkRepository<TrainingCourse>, ITrainingCourseRepository
{
    public EntityFrameworkTrainingCourseRepository(HrDbContext dbContext)
        : base(dbContext, course => course.Id)
    {
    }

    public async Task<IReadOnlyCollection<TrainingCourse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllInternalAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<TrainingCourse?> GetByIdAsync(Guid trainingCourseId, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(trainingCourseId, cancellationToken);
    }

    public Task<TrainingCourse> AddAsync(TrainingCourse trainingCourse, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(trainingCourse, cancellationToken);
    }

    public Task<TrainingCourse?> UpdateAsync(TrainingCourse trainingCourse, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(trainingCourse, cancellationToken);
    }

    public Task<bool> RemoveAsync(Guid trainingCourseId, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(trainingCourseId, cancellationToken);
    }
}
