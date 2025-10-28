using System.Collections.Concurrent;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

/// <summary>
///     In-memory repository implementation for vacancies.
/// </summary>
public sealed class InMemoryVacancyRepository : IVacancyRepository
{
    private readonly ConcurrentDictionary<Guid, Vacancy> _vacancies = new();

    public Task<IReadOnlyCollection<Vacancy>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Vacancy> snapshot = _vacancies.Values
            .OrderByDescending(vacancy => vacancy.PostedAtUtc)
            .ToArray();
        return Task.FromResult(snapshot);
    }

    public Task<Vacancy?> GetByIdAsync(Guid vacancyId, CancellationToken cancellationToken = default)
    {
        _vacancies.TryGetValue(vacancyId, out var vacancy);
        return Task.FromResult(vacancy);
    }

    public Task<Vacancy> AddAsync(Vacancy vacancy, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(vacancy);

        if (!_vacancies.TryAdd(vacancy.Id, vacancy))
        {
            throw new InvalidOperationException($"A vacancy with id '{vacancy.Id}' already exists.");
        }

        return Task.FromResult(vacancy);
    }

    public Task<Vacancy?> UpdateAsync(Vacancy vacancy, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(vacancy);

        if (!_vacancies.ContainsKey(vacancy.Id))
        {
            return Task.FromResult<Vacancy?>(null);
        }

        _vacancies[vacancy.Id] = vacancy;
        return Task.FromResult<Vacancy?>(vacancy);
    }
}
