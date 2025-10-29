namespace HR.Api.Contracts;

/// <summary>
///     Represents the collection of roles assigned to a specific user.
/// </summary>
public sealed record UserRolesResponse(Guid UserId, IReadOnlyCollection<string> Roles);
