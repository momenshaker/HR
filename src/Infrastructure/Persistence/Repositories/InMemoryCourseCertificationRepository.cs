using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

/// <summary>
///     In-memory repository implementation for course certifications.
/// </summary>
public sealed class InMemoryCourseCertificationRepository : ICourseCertificationRepository
{
    private readonly ConcurrentDictionary<Guid, CourseCertification> _certifications = new();

    public Task<CourseCertification?> GetByIdAsync(Guid certificationId, CancellationToken cancellationToken = default)
    {
        _certifications.TryGetValue(certificationId, out var certification);
        return Task.FromResult(certification);
    }

    public Task<CourseCertification?> GetByCertificateNumberAsync(string certificateNumber, CancellationToken cancellationToken = default)
    {
        var normalized = certificateNumber.ToUpperInvariant();
        var certification = _certifications.Values.FirstOrDefault(cert => cert.CertificateNumber == normalized);
        return Task.FromResult(certification);
    }

    public Task<IReadOnlyCollection<CourseCertification>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<CourseCertification> snapshot = _certifications.Values.Where(cert => cert.CourseId == courseId).ToList();
        return Task.FromResult(snapshot);
    }

    public Task<IReadOnlyCollection<CourseCertification>> GetByEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<CourseCertification> snapshot = _certifications.Values.Where(cert => cert.EmployeeId == employeeId).ToList();
        return Task.FromResult(snapshot);
    }

    public Task<CourseCertification> AddAsync(CourseCertification certification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(certification);

        if (!_certifications.TryAdd(certification.Id, certification))
        {
            throw new InvalidOperationException($"A certification with id '{certification.Id}' already exists.");
        }

        return Task.FromResult(certification);
    }

    public Task<CourseCertification?> UpdateAsync(CourseCertification certification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(certification);

        if (!_certifications.ContainsKey(certification.Id))
        {
            return Task.FromResult<CourseCertification?>(null);
        }

        _certifications[certification.Id] = certification;
        return Task.FromResult<CourseCertification?>(certification);
    }
}
