namespace HR.Api.Contracts;

/// <summary>
///     Response payload returned when registering a new user.
/// </summary>
public sealed record RegistrationResponse(Guid UserId, string? EmailConfirmationToken);
