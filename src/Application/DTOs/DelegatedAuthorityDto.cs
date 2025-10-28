namespace HR.Application.DTOs;

/// <summary>
///     Read model describing a delegated authority arrangement.
/// </summary>
public sealed record DelegatedAuthorityDto(
    Guid Id,
    Guid? GrantorEmployeeId,
    Guid? DelegateEmployeeId,
    Guid? GrantorPositionId,
    Guid? DelegatePositionId,
    string AuthorityScope,
    decimal? ApprovalLimit,
    DateTimeOffset GrantedOnUtc,
    DateTimeOffset? ExpiresOnUtc,
    DateTimeOffset? RevokedOnUtc,
    bool IsRevoked,
    string Notes);
