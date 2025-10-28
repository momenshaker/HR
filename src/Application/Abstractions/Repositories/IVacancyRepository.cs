using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository contract for managing <see cref="Vacancy" /> aggregates.
/// </summary>
public interface IVacancyRepository
{
    Task<IReadOnlyCollection<Vacancy>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Vacancy?> GetByIdAsync(Guid vacancyId, CancellationToken cancellationToken = default);

    Task<Vacancy> AddAsync(Vacancy vacancy, CancellationToken cancellationToken = default);

    Task<Vacancy?> UpdateAsync(Vacancy vacancy, CancellationToken cancellationToken = default);
}
