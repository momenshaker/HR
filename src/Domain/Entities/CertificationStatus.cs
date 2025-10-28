namespace HR.Domain.Entities;

/// <summary>
///     Represents the lifecycle state of an issued certification.
/// </summary>
public enum CertificationStatus
{
    Active = 0,
    Revoked = 1,
    Expired = 2
}
