using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HR.Infrastructure.Persistence.EntityFramework.Repositories;

internal sealed class EntityFrameworkCourseEnrollmentRepository : EntityFrameworkRepository<CourseEnrollment>, ICourseEnrollmentRepository
{
    public EntityFrameworkCourseEnrollmentRepository(HrDbContext dbContext)
        : base(dbContext, enrollment => enrollment.Id)
    {
    }

    public Task<CourseEnrollment?> GetByIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        return GetByIdInternalAsync(enrollmentId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<CourseEnrollment>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().Where(enrollment => enrollment.CourseId == courseId).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<CourseEnrollment>> GetByEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().Where(enrollment => enrollment.EmployeeId == employeeId).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<CourseEnrollment?> GetByCourseAndEmployeeAsync(Guid courseId, Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().FirstOrDefaultAsync(enrollment => enrollment.CourseId == courseId && enrollment.EmployeeId == employeeId, cancellationToken).ConfigureAwait(false);
    }

    public Task<CourseEnrollment> AddAsync(CourseEnrollment enrollment, CancellationToken cancellationToken = default)
    {
        return AddInternalAsync(enrollment, cancellationToken);
    }

    public Task<CourseEnrollment?> UpdateAsync(CourseEnrollment enrollment, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(enrollment, cancellationToken);
    }

    public Task<bool> RemoveAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        return RemoveInternalAsync(enrollmentId, cancellationToken);
    }
}
