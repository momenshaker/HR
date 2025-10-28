using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

/// <summary>
///     In-memory repository implementation for recognition programmes.
/// </summary>
public sealed class InMemoryRecognitionProgramRepository : IRecognitionProgramRepository
{
    private readonly ConcurrentDictionary<Guid, RecognitionProgram> _programs = new();

    public Task<IReadOnlyCollection<RecognitionProgram>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<RecognitionProgram> snapshot = _programs.Values.ToList();
        return Task.FromResult(snapshot);
    }

    public Task<RecognitionProgram?> GetByIdAsync(Guid programId, CancellationToken cancellationToken = default)
    {
        _programs.TryGetValue(programId, out var program);
        return Task.FromResult(program);
    }

    public Task<RecognitionProgram> AddAsync(RecognitionProgram program, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(program);

        if (!_programs.TryAdd(program.Id, program))
        {
            throw new InvalidOperationException($"A recognition programme with id '{program.Id}' already exists.");
        }

        return Task.FromResult(program);
    }

    public Task<RecognitionProgram?> UpdateAsync(RecognitionProgram program, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(program);

        if (!_programs.ContainsKey(program.Id))
        {
            return Task.FromResult<RecognitionProgram?>(null);
        }

        _programs[program.Id] = program;
        return Task.FromResult<RecognitionProgram?>(program);
    }

    public Task<bool> RemoveAsync(Guid programId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_programs.TryRemove(programId, out _));
    }
}
