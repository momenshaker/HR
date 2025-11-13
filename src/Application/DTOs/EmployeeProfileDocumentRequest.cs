using System;
using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     API request payload describing a profile document attached to an employee.
/// </summary>
public sealed class EmployeeProfileDocumentRequest
{
    public Guid? Id { get; init; }

    [Required]
    [MaxLength(260)]
    public string FileName { get; init; } = string.Empty;

    [Required]
    [MaxLength(512)]
    public string StoragePath { get; init; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; init; } = string.Empty;

    [MaxLength(100)]
    public string ContentType { get; init; } = string.Empty;

    public DateTimeOffset? UploadedAtUtc { get; init; }
}
