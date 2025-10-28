using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Request payload describing a compliance document maintained for an employee.
/// </summary>
public sealed class EmployeeComplianceDocumentRequest
{
    public Guid? Id { get; init; }

    [Required]
    [MaxLength(100)]
    public string DocumentType { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ReferenceNumber { get; init; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; init; } = string.Empty;

    [Required]
    public DateOnly IssuedOn { get; init; }

    public DateOnly? ExpiresOn { get; init; }

    [MaxLength(300)]
    public string StoragePath { get; init; } = string.Empty;
}
