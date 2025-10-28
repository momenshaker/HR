using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkPulseSurveyRepository
    : EntityFrameworkRepository<PulseSurvey>, IPulseSurveyRepository
{
    public EntityFrameworkPulseSurveyRepository(HrDbContext dbContext)
        : base(dbContext, survey => survey.Id)
    {
    }

    public async Task<IReadOnlyCollection<PulseSurvey>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllInternalAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<PulseSurvey?> GetByIdAsync(Guid surveyId, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(surveyId, cancellationToken);
    }

    public Task<PulseSurvey> AddAsync(PulseSurvey survey, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(survey, cancellationToken);
    }

    public Task<PulseSurvey?> UpdateAsync(PulseSurvey survey, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(survey, cancellationToken);
    }

    public Task<bool> RemoveAsync(Guid surveyId, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(surveyId, cancellationToken);
    }
}
