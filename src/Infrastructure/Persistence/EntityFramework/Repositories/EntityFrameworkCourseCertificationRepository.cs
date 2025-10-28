using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkCourseCertificationRepository : EntityFrameworkRepository<CourseCertification>, ICourseCertificationRepository
{
    public EntityFrameworkCourseCertificationRepository(HrDbContext dbContext)
        : base(dbContext, certification => certification.Id)
    {
    }

    public Task<CourseCertification?> GetByIdAsync(Guid certificationId, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(certificationId, cancellationToken);
    }

    public async Task<CourseCertification?> GetByCertificateNumberAsync(string certificateNumber, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().FirstOrDefaultAsync(certification => certification.CertificateNumber == certificateNumber, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<CourseCertification>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().Where(certification => certification.CourseId == courseId).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<CourseCertification>> GetByEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().Where(certification => certification.EmployeeId == employeeId).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<CourseCertification> AddAsync(CourseCertification certification, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(certification, cancellationToken);
    }

    public Task<CourseCertification?> UpdateAsync(CourseCertification certification, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(certification, cancellationToken);
    }
}
