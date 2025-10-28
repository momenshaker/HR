using System;

namespace HR.Domain.Entities;

/// <summary>
///     Represents a certification issued for completing a training course.
/// </summary>
public sealed class CourseCertification
{
    public Guid Id { get; init; }

    public Guid CourseId { get; init; }

    public Guid EmployeeId { get; init; }

    public string CertificateNumber { get; init; } = string.Empty;

    public DateOnly IssuedOn { get; init; }

    public DateOnly? ExpiresOn { get; init; }

    public string IssuedBy { get; init; } = string.Empty;

    public CertificationStatus Status { get; init; }

    public string GovernanceNotes { get; init; } = string.Empty;
}
