using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository abstraction for organisation positions.
/// </summary>
public interface IPositionRepository
{
    Task<IReadOnlyCollection<Position>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Position?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Position>> GetByOrganizationUnitAsync(Guid organizationUnitId, CancellationToken cancellationToken = default);

    Task<Position> AddAsync(Position position, CancellationToken cancellationToken = default);

    Task<Position?> UpdateAsync(Position position, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default);
}
