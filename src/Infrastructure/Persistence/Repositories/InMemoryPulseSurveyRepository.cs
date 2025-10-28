using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

/// <summary>
///     In-memory repository implementation for pulse surveys.
/// </summary>
public sealed class InMemoryPulseSurveyRepository : IPulseSurveyRepository
{
    private readonly ConcurrentDictionary<Guid, PulseSurvey> _surveys = new();

    public Task<IReadOnlyCollection<PulseSurvey>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<PulseSurvey> snapshot = _surveys.Values.ToList();
        return Task.FromResult(snapshot);
    }

    public Task<PulseSurvey?> GetByIdAsync(Guid surveyId, CancellationToken cancellationToken = default)
    {
        _surveys.TryGetValue(surveyId, out var survey);
        return Task.FromResult(survey);
    }

    public Task<PulseSurvey> AddAsync(PulseSurvey survey, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(survey);

        if (!_surveys.TryAdd(survey.Id, survey))
        {
            throw new InvalidOperationException($"A pulse survey with id '{survey.Id}' already exists.");
        }

        return Task.FromResult(survey);
    }

    public Task<PulseSurvey?> UpdateAsync(PulseSurvey survey, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(survey);

        if (!_surveys.ContainsKey(survey.Id))
        {
            return Task.FromResult<PulseSurvey?>(null);
        }

        _surveys[survey.Id] = survey;
        return Task.FromResult<PulseSurvey?>(survey);
    }

    public Task<bool> RemoveAsync(Guid surveyId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_surveys.TryRemove(surveyId, out _));
    }
}
