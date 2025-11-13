using System;

namespace HR.Domain.Entities;

/// <summary>
///     Represents a binary artefact stored against an employee profile.
/// </summary>
public sealed class EmployeeProfileDocument
{
    public Guid Id { get; init; }

    public Guid EmployeeId { get; init; }

    public string FileName { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string StoragePath { get; init; } = string.Empty;

    public string ContentType { get; init; } = string.Empty;

    public DateTimeOffset UploadedAtUtc { get; init; }

    public Employee? Employee { get; init; }
}
