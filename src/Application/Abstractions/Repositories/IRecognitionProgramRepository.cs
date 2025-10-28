using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository contract for managing <see cref="RecognitionProgram" /> aggregates.
/// </summary>
public interface IRecognitionProgramRepository
{
    Task<IReadOnlyCollection<RecognitionProgram>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<RecognitionProgram?> GetByIdAsync(Guid programId, CancellationToken cancellationToken = default);

    Task<RecognitionProgram> AddAsync(RecognitionProgram program, CancellationToken cancellationToken = default);

    Task<RecognitionProgram?> UpdateAsync(RecognitionProgram program, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid programId, CancellationToken cancellationToken = default);
}
