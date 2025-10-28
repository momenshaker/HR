using System.Collections.Generic;

namespace HR.Application.DTOs;

/// <summary>
///     Read model describing a self-service account.
/// </summary>
public sealed record SelfServiceAccountDto(
    Guid Id,
    Guid EmployeeId,
    string Email,
    string OAuthProvider,
    string ExternalIdentifier,
    bool IsMfaEnabled,
    bool IsLocked,
    DateTimeOffset CreatedOnUtc,
    DateTimeOffset? UpdatedOnUtc,
    DateTimeOffset? LastSignInUtc,
    IReadOnlyCollection<string> FeatureAccess);
