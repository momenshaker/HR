using System;

namespace HR.Domain.Entities;

/// <summary>
///     A scheduled session for a course.
/// </summary>
public sealed class CourseSession
{
    public Guid Id { get; init; }

    public Guid CourseId { get; init; }

    public DateTime StartUtc { get; init; }

    public DateTime EndUtc { get; init; }

    public string? Location { get; init; }

    public string? MeetingUrl { get; init; }

    public int? Capacity { get; init; }
}

