namespace HR.Api.Contracts;

/// <summary>
///     Represents a token returned to the caller for email confirmation or password reset flows.
/// </summary>
public sealed record TokenResponse(Guid UserId, string Token, string TokenType);
