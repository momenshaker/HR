using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Represents a lookup value returned to clients.
/// </summary>
public sealed record LookupValueDto(
    Guid Id,
    string Category,
    string Code,
    string DisplayName,
    string? Description,
    int SortOrder,
    bool IsActive,
    DateTime UpdatedAtUtc);

/// <summary>
///     Group of lookup values under a common category.
/// </summary>
public sealed record LookupCategoryDto(string Category, IReadOnlyCollection<LookupValueDto> Values);

/// <summary>
///     Lookup collection response paired with a version token used for caching.
/// </summary>
public sealed record LookupCollectionDto(string VersionToken, IReadOnlyCollection<LookupCategoryDto> Categories);

/// <summary>
///     Request payload for creating a lookup value.
/// </summary>
public sealed class CreateLookupValueRequest : IValidatableRequest
{
    [Required]
    [MaxLength(100)]
    public string Category { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Code { get; init; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string DisplayName { get; init; } = string.Empty;

    [MaxLength(512)]
    public string? Description { get; init; }

    [Range(0, 10_000)]
    public int? SortOrder { get; init; }

    public bool IsActive { get; init; } = true;
}

/// <summary>
///     Request payload for updating a lookup value.
/// </summary>
public sealed class UpdateLookupValueRequest : IValidatableRequest
{
    [Required]
    [MaxLength(100)]
    public string Category { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Code { get; init; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string DisplayName { get; init; } = string.Empty;

    [MaxLength(512)]
    public string? Description { get; init; }

    [Range(0, 10_000)]
    public int? SortOrder { get; init; }

    public bool IsActive { get; init; } = true;
}
