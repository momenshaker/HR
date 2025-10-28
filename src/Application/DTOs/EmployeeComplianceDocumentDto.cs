namespace HR.Application.DTOs;

/// <summary>
///     Read model representing a compliance document stored against an employee master record.
/// </summary>
public sealed record EmployeeComplianceDocumentDto(
    Guid Id,
    string DocumentType,
    string ReferenceNumber,
    string Status,
    DateOnly IssuedOn,
    DateOnly? ExpiresOn,
    string StoragePath);
