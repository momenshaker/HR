using System;

namespace HR.Domain.Entities;

/// <summary>
///     Represents a delegation of authority between organisational leaders.
/// </summary>
public sealed class DelegatedAuthority
{
    public Guid Id { get; init; }

    public Guid? GrantorEmployeeId { get; init; }

    public Guid? DelegateEmployeeId { get; init; }

    public Guid? GrantorPositionId { get; init; }

    public Guid? DelegatePositionId { get; init; }

    public string AuthorityScope { get; init; } = string.Empty;

    public decimal? ApprovalLimit { get; init; }

    public DateTimeOffset GrantedOnUtc { get; init; }

    public DateTimeOffset? ExpiresOnUtc { get; init; }

    public DateTimeOffset? RevokedOnUtc { get; init; }

    public bool IsRevoked { get; init; }

    public string Notes { get; init; } = string.Empty;
}
