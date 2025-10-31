using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

public sealed record LiteCourseDto(
    Guid Id,
    Guid OrganizationId,
    string Code,
    string Title,
    string? Description,
    decimal DurationHours,
    bool IsMandatory);

public sealed class CreateLiteCourseRequest
{
    [Required]
    public Guid OrganizationId { get; init; }

    [Required]
    [MaxLength(50)]
    public string Code { get; init; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Title { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; init; }

    [Range(typeof(decimal), "0", "99999")]
    public decimal DurationHours { get; init; }

    public bool IsMandatory { get; init; }
}

public sealed record LiteCourseSessionDto(
    Guid Id,
    Guid CourseId,
    DateTime StartUtc,
    DateTime EndUtc,
    string? Location,
    string? MeetingUrl,
    int? Capacity);

public sealed class CreateLiteCourseSessionRequest
{
    [Required]
    public Guid CourseId { get; init; }

    [Required]
    public DateTime StartUtc { get; init; }

    [Required]
    public DateTime EndUtc { get; init; }

    [MaxLength(500)]
    public string? Location { get; init; }

    [MaxLength(1000)]
    public string? MeetingUrl { get; init; }

    [Range(1, 100000)]
    public int? Capacity { get; init; }
}

public enum LiteEnrollmentStatus
{
    Enrolled = 0,
    Completed = 1,
    Cancelled = 2
}

public sealed record LiteEnrollmentDto(
    Guid SessionId,
    Guid EmployeeId,
    DateTime EnrolledAtUtc,
    LiteEnrollmentStatus Status,
    decimal? Score,
    string? CertificateUrl);

