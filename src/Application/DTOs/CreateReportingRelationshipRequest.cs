using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for creating a reporting relationship.
/// </summary>
public sealed class CreateReportingRelationshipRequest
{
    [Required]
    public Guid ManagerPositionId { get; init; }

    [Required]
    public Guid ReportPositionId { get; init; }

    [Required]
    [MaxLength(50)]
    public string RelationshipType { get; init; } = string.Empty;

    public DateOnly? EffectiveFrom { get; init; }

    public DateOnly? EffectiveTo { get; init; }

    public bool IsPrimary { get; init; }
}
