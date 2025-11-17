using System;

namespace HR.Domain.Entities;

/// <summary>
///     Represents an employment offer generated for an application.
/// </summary>
public sealed class Offer
{
    public Guid Id { get; init; }

    public Guid ApplicationId { get; init; }

    public string PositionTitle { get; init; } = string.Empty;

    public string EmploymentType { get; init; } = string.Empty;

    public decimal? ProposedSalary { get; init; }

    public string Currency { get; init; } = string.Empty;

    public DateTime? StartDate { get; init; }

    public int? ProbationPeriodMonths { get; init; }

    public string Status { get; init; } = string.Empty;

    public string OfferDocumentUrl { get; init; } = string.Empty;

    public string Comments { get; init; } = string.Empty;
}
