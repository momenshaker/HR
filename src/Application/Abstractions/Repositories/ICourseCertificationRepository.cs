using HR.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HR.Application.Abstractions.Repositories;

/// <summary>
///     Repository contract for managing course certification aggregates.
/// </summary>
public interface ICourseCertificationRepository
{
    Task<CourseCertification?> GetByIdAsync(Guid certificationId, CancellationToken cancellationToken = default);

    Task<CourseCertification?> GetByCertificateNumberAsync(string certificateNumber, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CourseCertification>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CourseCertification>> GetByEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<CourseCertification> AddAsync(CourseCertification certification, CancellationToken cancellationToken = default);

    Task<CourseCertification?> UpdateAsync(CourseCertification certification, CancellationToken cancellationToken = default);
}
