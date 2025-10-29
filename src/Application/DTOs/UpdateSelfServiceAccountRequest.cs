using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming payload for updating a self-service account.
/// </summary>
public sealed class UpdateSelfServiceAccountRequest : IValidatableRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string OAuthProvider { get; init; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string ExternalIdentifier { get; init; } = string.Empty;

    public bool IsMfaEnabled { get; init; }

    public bool IsLocked { get; init; }

    public DateTimeOffset? LastSignInUtc { get; init; }

    public IReadOnlyCollection<string> FeatureAccess { get; init; } = Array.Empty<string>();
}