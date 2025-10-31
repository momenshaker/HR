using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HR.Domain.Entities;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository contract for managing <see cref="Department" /> aggregates.
/// </summary>
public interface IDepartmentRepository
{
    Task<IReadOnlyCollection<Department>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Department?> GetByIdAsync(Guid departmentId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Department>> GetByIdsAsync(
        IReadOnlyCollection<Guid> departmentIds,
        CancellationToken cancellationToken = default);

    Task<Department> AddAsync(Department department, CancellationToken cancellationToken = default);

    Task<Department?> UpdateAsync(Department department, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid departmentId, CancellationToken cancellationToken = default);
}
