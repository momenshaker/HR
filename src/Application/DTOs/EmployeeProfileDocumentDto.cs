using System;

namespace HR.Application.DTOs;

/// <summary>
///     Read model representing an employee profile document.
/// </summary>
public sealed record EmployeeProfileDocumentDto(
    Guid Id,
    string FileName,
    string Description,
    string StoragePath,
    string ContentType,
    DateTimeOffset UploadedAtUtc);
