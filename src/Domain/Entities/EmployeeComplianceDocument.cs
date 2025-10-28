namespace HR.Domain.Entities;

/// <summary>
///     Represents a compliance artefact stored against an employee master record.
/// </summary>
public sealed class EmployeeComplianceDocument
{
    /// <summary>
    ///     Gets the unique identifier for the document record.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    ///     Gets the classification of document (e.g. ID Proof, Work Permit, Certification).
    /// </summary>
    public string DocumentType { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the document reference or certificate number for audit trails.
    /// </summary>
    public string ReferenceNumber { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the lifecycle status for the compliance document (e.g. Verified, Pending, Expired).
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the issue date of the document.
    /// </summary>
    public DateOnly IssuedOn { get; init; }

    /// <summary>
    ///     Gets the optional expiry date for time-bound documents.
    /// </summary>
    public DateOnly? ExpiresOn { get; init; }

    /// <summary>
    ///     Gets a logical storage path or URI pointing to the persisted document binary.
    /// </summary>
    public string StoragePath { get; init; } = string.Empty;
}
