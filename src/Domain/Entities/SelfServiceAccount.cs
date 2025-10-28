using System;
using System.Collections.Generic;

namespace HR.Domain.Entities;

/// <summary>
///     Represents an employee-facing self-service account with OAuth integration metadata.
/// </summary>
public sealed class SelfServiceAccount
{
    public Guid Id { get; init; }

    public Guid EmployeeId { get; init; }

    public string Email { get; init; } = string.Empty;

    public string OAuthProvider { get; init; } = string.Empty;

    public string ExternalIdentifier { get; init; } = string.Empty;

    public bool IsMfaEnabled { get; init; }

    public bool IsLocked { get; init; }

    public DateTimeOffset CreatedOnUtc { get; init; }

    public DateTimeOffset? UpdatedOnUtc { get; init; }

    public DateTimeOffset? LastSignInUtc { get; init; }

    public List<string> FeatureAccess { get; init; } = new();
}
