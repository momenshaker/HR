using System;

namespace HR.Domain.Entities;

/// <summary>
///     Lightweight training course definition (per organization).
/// </summary>
public sealed class Course
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public string Code { get; init; } = string.Empty; // Unique per org

    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public decimal DurationHours { get; init; } // decimal(5,2)

    public bool IsMandatory { get; init; }
}

