using HR.Application.DTOs;
using HR.Domain.Entities;
using System;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="CourseCertification" /> entities.
/// </summary>
public static class CourseCertificationMappings
{
    public static CourseCertificationDto ToDto(this CourseCertification certification)
    {
        ArgumentNullException.ThrowIfNull(certification);

        return new CourseCertificationDto(
            certification.Id,
            certification.CourseId,
            certification.EmployeeId,
            certification.CertificateNumber,
            certification.IssuedOn,
            certification.ExpiresOn,
            certification.IssuedBy,
            certification.Status,
            certification.GovernanceNotes);
    }

    public static CourseCertification ToEntity(this IssueCourseCertificationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var certificateNumber = request.CertificateNumber?.Trim();
        if (string.IsNullOrWhiteSpace(certificateNumber))
        {
            throw new ArgumentException("Certificate number is required.", nameof(request.CertificateNumber));
        }

        var issuedBy = request.IssuedBy?.Trim();
        if (string.IsNullOrWhiteSpace(issuedBy))
        {
            throw new ArgumentException("Issued by is required.", nameof(request.IssuedBy));
        }

        var governanceNotes = request.GovernanceNotes?.Trim() ?? string.Empty;

        return new CourseCertification
        {
            Id = Guid.NewGuid(),
            CourseId = request.CourseId,
            EmployeeId = request.EmployeeId,
            CertificateNumber = certificateNumber.ToUpperInvariant(),
            IssuedOn = request.IssuedOn,
            ExpiresOn = request.ExpiresOn,
            IssuedBy = issuedBy,
            Status = CertificationStatus.Active,
            GovernanceNotes = governanceNotes
        };
    }

    public static CourseCertification ApplyGovernanceUpdate(
        this CourseCertification existing,
        CertificationStatus status,
        string governanceNotes)
    {
        ArgumentNullException.ThrowIfNull(existing);

        var notes = governanceNotes?.Trim() ?? string.Empty;

        return new CourseCertification
        {
            Id = existing.Id,
            CourseId = existing.CourseId,
            EmployeeId = existing.EmployeeId,
            CertificateNumber = existing.CertificateNumber,
            IssuedOn = existing.IssuedOn,
            ExpiresOn = existing.ExpiresOn,
            IssuedBy = existing.IssuedBy,
            Status = status,
            GovernanceNotes = notes
        };
    }
}
