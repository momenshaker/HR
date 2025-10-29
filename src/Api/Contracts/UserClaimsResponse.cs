namespace HR.Api.Contracts;

/// <summary>
///     Represents the claims associated with a user.
/// </summary>
public sealed record UserClaimsResponse(Guid UserId, IReadOnlyDictionary<string, string> Claims);
