using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class TrainingService : ITrainingService
{
    private readonly ITrainingCourseRepository _courseRepository;

    public TrainingService(ITrainingCourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<TrainingCourseDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var courses = await _courseRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return courses.Select(course => course.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<TrainingCourseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await _courseRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return course?.ToDto();
    }

    /// <inheritdoc />
    public async Task<TrainingCourseDto> CreateAsync(CreateTrainingCourseRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = request.ToEntity();
        var created = await _courseRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<TrainingCourseDto?> UpdateAsync(Guid id, UpdateTrainingCourseRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _courseRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updatedEntity = request.ApplyUpdates(existing);
        var persisted = await _courseRepository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);

        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _courseRepository.RemoveAsync(id, cancellationToken);
    }
}
