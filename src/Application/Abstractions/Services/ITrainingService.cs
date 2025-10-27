using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service responsible for training and development operations.
/// </summary>
public interface ITrainingService
{
    Task<IReadOnlyCollection<TrainingCourseDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<TrainingCourseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TrainingCourseDto> CreateAsync(CreateTrainingCourseRequest request, CancellationToken cancellationToken = default);

    Task<TrainingCourseDto?> UpdateAsync(Guid id, UpdateTrainingCourseRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
