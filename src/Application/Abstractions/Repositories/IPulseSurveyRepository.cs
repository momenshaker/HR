using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository contract for managing <see cref="PulseSurvey" /> aggregates.
/// </summary>
public interface IPulseSurveyRepository
{
    Task<IReadOnlyCollection<PulseSurvey>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PulseSurvey?> GetByIdAsync(Guid surveyId, CancellationToken cancellationToken = default);

    Task<PulseSurvey> AddAsync(PulseSurvey survey, CancellationToken cancellationToken = default);

    Task<PulseSurvey?> UpdateAsync(PulseSurvey survey, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid surveyId, CancellationToken cancellationToken = default);
}
