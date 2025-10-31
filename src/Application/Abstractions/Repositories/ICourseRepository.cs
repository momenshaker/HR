using HR.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Lightweight course repository.
/// </summary>
public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Course?> GetByOrgAndCodeAsync(Guid organizationId, string code, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Course>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);

    Task<Course> AddAsync(Course course, CancellationToken cancellationToken = default);
}

