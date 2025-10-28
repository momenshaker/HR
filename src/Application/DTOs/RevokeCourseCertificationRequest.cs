using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Request payload for revoking an issued certification.
/// </summary>
public sealed class RevokeCourseCertificationRequest
{
    [MaxLength(2000)]
    public string GovernanceNotes { get; init; } = string.Empty;
}
