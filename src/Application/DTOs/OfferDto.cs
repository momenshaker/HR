using System;

namespace HR.Application.DTOs;

/// <summary>
///     Read model describing an employment offer for an application.
/// </summary>
public sealed record OfferDto(
    Guid Id,
    Guid ApplicationId,
    string PositionTitle,
    string EmploymentType,
    decimal? ProposedSalary,
    string Currency,
    DateTime? StartDate,
    int? ProbationPeriodMonths,
    string Status,
    string OfferDocumentUrl,
    string Comments);
