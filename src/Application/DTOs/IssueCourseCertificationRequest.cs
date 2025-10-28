using System;
using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Request payload for issuing a course certification.
/// </summary>
public sealed class IssueCourseCertificationRequest
{
    [Required]
    public Guid CourseId { get; init; }

    [Required]
    public Guid EmployeeId { get; init; }

    [Required]
    [MaxLength(100)]
    public string CertificateNumber { get; init; } = string.Empty;

    [Required]
    public DateOnly IssuedOn { get; init; }

    public DateOnly? ExpiresOn { get; init; }

    [Required]
    [MaxLength(150)]
    public string IssuedBy { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string GovernanceNotes { get; init; } = string.Empty;
}
