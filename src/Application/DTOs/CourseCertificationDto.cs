using HR.Domain.Entities;
using System;

namespace HR.Application.DTOs;

/// <summary>
///     Read model describing an issued course certification.
/// </summary>
public sealed record CourseCertificationDto(
    Guid Id,
    Guid CourseId,
    Guid EmployeeId,
    string CertificateNumber,
    DateOnly IssuedOn,
    DateOnly? ExpiresOn,
    string IssuedBy,
    CertificationStatus Status,
    string GovernanceNotes);
