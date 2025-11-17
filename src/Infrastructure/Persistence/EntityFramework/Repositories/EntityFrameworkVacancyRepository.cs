using System.Linq;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkVacancyRepository : EntityFrameworkRepository<Vacancy>, IVacancyRepository
{
    public EntityFrameworkVacancyRepository(HrDbContext dbContext)
        : base(dbContext, vacancy => vacancy.Id)
    {
    }

    public async Task<IReadOnlyCollection<Vacancy>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var vacancies = await GetAllInternalAsync(cancellationToken).ConfigureAwait(false);
        return vacancies
            .OrderByDescending(vacancy => vacancy.PublishedAtUtc ?? vacancy.CreatedAtUtc)
            .ToArray();
    }

    public Task<Vacancy?> GetByIdAsync(Guid vacancyId, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(vacancyId, cancellationToken);
    }

    public Task<Vacancy> AddAsync(Vacancy vacancy, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(vacancy, cancellationToken);
    }

    public Task<Vacancy?> UpdateAsync(Vacancy vacancy, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(vacancy, cancellationToken);
    }
}
