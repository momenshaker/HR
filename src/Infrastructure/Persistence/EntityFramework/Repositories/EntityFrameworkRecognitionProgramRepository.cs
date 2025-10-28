using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkRecognitionProgramRepository
    : EntityFrameworkRepository<RecognitionProgram>, IRecognitionProgramRepository
{
    public EntityFrameworkRecognitionProgramRepository(HrDbContext dbContext)
        : base(dbContext, program => program.Id)
    {
    }

    public async Task<IReadOnlyCollection<RecognitionProgram>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllInternalAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<RecognitionProgram?> GetByIdAsync(Guid programId, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(programId, cancellationToken);
    }

    public Task<RecognitionProgram> AddAsync(RecognitionProgram program, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(program, cancellationToken);
    }

    public Task<RecognitionProgram?> UpdateAsync(RecognitionProgram program, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(program, cancellationToken);
    }

    public Task<bool> RemoveAsync(Guid programId, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(programId, cancellationToken);
    }
}
